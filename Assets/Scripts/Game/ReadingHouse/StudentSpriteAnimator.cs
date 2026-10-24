using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The student's 2D look in the 3D house. Presentation only: it reads where
    /// the existing walker / mover put the student and shows the matching artwork.
    ///
    ///   • Follows the student's root position; never parented to it, so the root's
    ///     own rotation and scale can never rotate, stretch or enlarge the sprite.
    ///   • The sprite plane turns about the vertical axis to face the camera (yaw
    ///     only — it never tilts); the ARTWORK is chosen from the direction he
    ///     moves relative to the camera (walk right / left mirrored / toward / away).
    ///   • Smooth playback:
    ///       – walks advance by DISTANCE walked (one 8-frame cycle = one stride
    ///         pair), so the steps match his speed over the floor — no skating,
    ///         no frantic legs;
    ///       – every frame cross-fades into the next through a second renderer
    ///         (full blend on idles, a short blend on walks so legs don't ghost);
    ///       – a gentle step bob / breathing lift is added by moving the sprite a
    ///         few millimetres — his size never changes.
    ///   • Standing still keeps the last facing; at the reading spot he faces the
    ///     dashboard (back view), turning round with the turn frames.
    /// The original 3D student meshes are hidden (not deleted) while this is on.
    /// </summary>
    public sealed class StudentSpriteAnimator : MonoBehaviour
    {
        public enum Facing { Front, Back, Right, Left }

        [Header("Wiring")]
        public Transform student;
        public Camera viewCamera;
        public ReadingHouseStudentMover mover;
        public StudentSpriteSet set;
        public SpriteRenderer spriteRenderer;
        [Tooltip("Second renderer used to fade into the next frame (created automatically if empty).")]
        public SpriteRenderer blendRenderer;
        public Transform shadow;

        [Header("Timing")]
        [Tooltip("Metres walked per full walk cycle (two steps), whatever its frame count.")]
        public float strideCycleMetres = 1.15f;
        public float idleFps = 8f;
        public float turnFps = 14f;
        public float pointFps = 20f;
        [Range(0f, 1f), Tooltip("Share of each walk frame spent fading into the next.")]
        public float walkBlend = 0.25f;
        [Range(0f, 1f), Tooltip("Share of each idle frame spent fading into the next.")]
        public float idleBlend = 1f;

        [Header("Feel")]
        public float moveThreshold = 0.2f;
        [Tooltip("A new walking direction must hold this long before the artwork switches (no flicker on diagonals).")]
        public float directionHold = 0.15f;
        public float stepBob = 0.035f;
        public float breathLift = 0.008f;
        public Vector2 blinkEvery = new Vector2(3f, 6f);
        [Tooltip("Seconds one blink takes (half, closed, closed, half).")]
        public float blinkTime = 0.22f;
        [Tooltip("Seconds standing facing the camera before he first smiles.")]
        public float smileAfter = 1.5f;
        public Vector2 smileEvery = new Vector2(7f, 12f);
        public float smileHold = 2.2f;
        public float smileFade = 0.3f;

        [Tooltip("Stand the sprite on the floor found under him (his controller root floats a few cm above it).")]
        public bool groundToFloor = true;
        [Tooltip("How far the feet press into the floor (m), so the soles sit on it rather than hover.")]
        public float footSink = 0.012f;

        [Tooltip("Hide him for a moment (e.g. while the camera faces the chalkboard). Position and state keep updating.")]
        public bool hidden;

        [Header("Fallback")]
        [Tooltip("Off = show the original 3D student instead.")]
        public bool use2D = true;

        public Facing CurrentFacing { get; private set; } = Facing.Front;
        public bool IsWalking { get; private set; }

        Renderer[] model3D;
        Vector3 lastPos;
        Facing pendingFacing;
        float pendingSince;
        StudentSpriteSet.Clip clip;
        float phase;                 // continuous frame position within the clip
        bool playingTurn, turnReversed;
        StudentSpriteSet.Clip oneShot;
        float nextBlink, blinkUntil;
        float smoothedSpeed;
        float groundOffset;
        bool groundKnown;

        void OnEnable()
        {
            if (student == null) return;
            model3D = student.GetComponentsInChildren<Renderer>(true);
            EnsureBlendRenderer();
            Apply3DVisibility();
            lastPos = student.position;
            transform.position = student.position;
            ScheduleBlink();
        }

        void OnDisable()
        {
            if (model3D != null) foreach (var r in model3D) if (r != null) r.enabled = true;
        }

        void EnsureBlendRenderer()
        {
            if (blendRenderer != null || spriteRenderer == null) return;
            var go = new GameObject("Blend Frame");
            go.transform.SetParent(spriteRenderer.transform, false);
            blendRenderer = go.AddComponent<SpriteRenderer>();
            blendRenderer.sharedMaterial = spriteRenderer.sharedMaterial;
            blendRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
            blendRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            blendRenderer.receiveShadows = false;
        }

        void Apply3DVisibility()
        {
            foreach (var r in model3D) if (r != null) r.enabled = !use2D;
            if (spriteRenderer != null) spriteRenderer.enabled = use2D;
            if (blendRenderer != null) blendRenderer.enabled = use2D;
            if (shadow != null) shadow.gameObject.SetActive(use2D);
        }

        /// <summary>Play a non-looping clip once (e.g. point at a stone), then return to idle.</summary>
        public void PlayOnce(StudentSpriteSet.Clip c)
        {
            if (c == null || c.frames == null || c.frames.Length == 0) return;
            oneShot = c; phase = 0f;
        }

        void LateUpdate()
        {
            if (student == null || set == null || spriteRenderer == null) return;
            float dt = Mathf.Max(Time.deltaTime, 1e-4f);

            // Follow the student's feet, stood on the floor under him; face the camera (yaw only).
            Vector3 pos = student.position;
            Vector3 feet = pos + Vector3.up * Ground(pos, dt);
            Transform cam = viewCamera != null ? viewCamera.transform : null;
            Vector3 camFwd = cam != null ? Vector3.ProjectOnPlane(pos - cam.position, Vector3.up) : Vector3.forward;
            if (camFwd.sqrMagnitude < 1e-6f) camFwd = Vector3.forward;
            camFwd.Normalize();
            transform.rotation = Quaternion.LookRotation(camFwd, Vector3.up);
            if (shadow != null) { shadow.position = feet + Vector3.up * (footSink + 0.012f); shadow.rotation = Quaternion.LookRotation(Vector3.down, camFwd); }

            // Movement relative to the camera (smoothed so one jerky frame can't flip the artwork).
            Vector3 v = Vector3.ProjectOnPlane(pos - lastPos, Vector3.up) / dt;
            float stepDistance = Vector3.ProjectOnPlane(pos - lastPos, Vector3.up).magnitude;
            lastPos = pos;
            smoothedSpeed = Mathf.Lerp(smoothedSpeed, v.magnitude, 1f - Mathf.Exp(-12f * dt));
            IsWalking = smoothedSpeed > moveThreshold;

            Facing want = CurrentFacing;
            if (IsWalking && v.sqrMagnitude > 0.01f)
            {
                Vector3 right = Vector3.Cross(Vector3.up, camFwd);
                float x = Vector3.Dot(v, right), z = Vector3.Dot(v, camFwd);
                want = Mathf.Abs(x) > Mathf.Abs(z) * 0.9f ? (x > 0 ? Facing.Right : Facing.Left) : (z > 0 ? Facing.Back : Facing.Front);
            }
            else if (!IsWalking && AtReadingSpot(pos)) want = Facing.Back;                 // reading: face the dashboard
            else if (!IsWalking && (CurrentFacing == Facing.Right || CurrentFacing == Facing.Left)) want = Facing.Front;

            // A reversal while walking turns through the side view first (never a snap from one side to the other).
            if (IsWalking && Opposite(want, CurrentFacing)) want = Between(CurrentFacing, v, camFwd);

            if (want != CurrentFacing)
            {
                if (want != pendingFacing) { pendingFacing = want; pendingSince = Time.time; }
                if (Time.time - pendingSince >= (IsWalking ? directionHold : 0.05f))
                {
                    if (!IsWalking && ((CurrentFacing == Facing.Front && want == Facing.Back) || (CurrentFacing == Facing.Back && want == Facing.Front)))
                    { playingTurn = true; turnReversed = want == Facing.Front; phase = 0f; }
                    CurrentFacing = want;
                }
            }
            else pendingFacing = want;

            // Choose the clip.
            StudentSpriteSet.Clip next;
            bool flip = false, walking = false;
            if (oneShot != null) next = oneShot;
            else if (IsWalking)
            {
                playingTurn = false; walking = true;
                switch (CurrentFacing)
                {
                    case Facing.Back: next = set.walkAway; break;
                    case Facing.Front: next = set.walkToward; break;
                    default: next = set.walkRight; flip = CurrentFacing == Facing.Left; break;
                }
            }
            else if (playingTurn) next = set.turnToBack;
            else next = CurrentFacing == Facing.Back ? set.idleBack : set.idleFront;

            if (next != clip)
            {
                // Walk ↔ walk keeps its stride phase (no stutter when the direction changes); anything else restarts.
                bool keepPhase = walking && clip != null && (clip == set.walkAway || clip == set.walkToward || clip == set.walkRight);
                clip = next;
                if (!keepPhase) phase = 0f;
            }

            if (clip != set.idleFront) { frontSince = -1f; smileWeight = 0f; }   // expressions start afresh each time he faces you

            // Advance.
            int n = clip.frames.Length;
            if (walking) phase += stepDistance / Mathf.Max(0.1f, strideCycleMetres) * n;
            else if (clip == set.turnToBack) phase += dt * turnFps;
            else if (clip == oneShot) phase += dt * pointFps;
            else phase += dt * idleFps;

            int a, b; float t;
            if (!clip.loop)
            {
                if (phase >= n - 1)
                {
                    phase = n - 1;
                    if (clip == oneShot) oneShot = null;
                    if (clip == set.turnToBack && playingTurn) playingTurn = false;
                }
                a = Mathf.FloorToInt(phase); b = Mathf.Min(a + 1, n - 1); t = phase - a;
                if (clip == set.turnToBack && turnReversed) { a = n - 1 - a; b = n - 1 - b; }
            }
            else if (clip == set.idleFront && !set.idlesAreCycles)
            {
                IdleFront(out a, out b, out t);
            }
            else if (clip == set.idleBack && !set.idlesAreCycles)
            {
                // breathe back and forth so the loop never jumps from the last pose to the first
                float p = Mathf.PingPong(phase, n - 1);
                a = Mathf.Clamp(Mathf.FloorToInt(p), 0, n - 1); b = Mathf.Min(a + 1, n - 1); t = p - a;
            }
            else
            {
                phase = Mathf.Repeat(phase, n);
                a = Mathf.FloorToInt(phase) % n; b = (a + 1) % n; t = phase - Mathf.Floor(phase);
            }

            // Cross-fade: a short window at the end of each walk frame, the whole frame on idles.
            float window = walking ? walkBlend : idleBlend;
            float w = window <= 0f ? 0f : Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(1f - window, 1f, t));
            spriteRenderer.sprite = clip.frames[a];
            spriteRenderer.flipX = flip;
            if (blendRenderer != null)
            {
                blendRenderer.sprite = clip.frames[b];
                blendRenderer.flipX = flip;
                blendRenderer.color = new Color(1f, 1f, 1f, w);
                blendRenderer.enabled = use2D && w > 0.01f;
            }

            if (clip == set.idleFront && set.idlesAreCycles) FrontFace(a, n, flip);

            // Gentle lift (position only): two bobs per walk cycle, slow breathing when idle.
            float lift = set.framesIncludeBob ? 0f : walking
                ? Mathf.Abs(Mathf.Sin(phase / n * Mathf.PI * 2f)) * stepBob
                : (Mathf.Sin(Time.time * 1.6f) * 0.5f + 0.5f) * breathLift;
            transform.position = feet + Vector3.up * lift;
            if (hidden)
            {
                spriteRenderer.enabled = false;
                if (blendRenderer != null) blendRenderer.enabled = false;
            }
            else spriteRenderer.enabled = use2D;
            if (shadow != null) shadow.gameObject.SetActive(use2D && !hidden);
        }

        static bool Opposite(Facing a, Facing b) =>
            (a == Facing.Left && b == Facing.Right) || (a == Facing.Right && b == Facing.Left) ||
            (a == Facing.Front && b == Facing.Back) || (a == Facing.Back && b == Facing.Front);

        /// <summary>The view between two opposite facings, on the side he is actually curving towards.</summary>
        static Facing Between(Facing from, Vector3 v, Vector3 camFwd)
        {
            Vector3 right = Vector3.Cross(Vector3.up, camFwd);
            if (from == Facing.Left || from == Facing.Right) return Vector3.Dot(v, camFwd) > 0f ? Facing.Back : Facing.Front;
            return Vector3.Dot(v, right) > 0f ? Facing.Right : Facing.Left;
        }

        /// <summary>Height of the floor under him relative to his root (smoothed; 0 when nothing is found).</summary>
        float Ground(Vector3 pos, float dt)
        {
            if (!groundToFloor) return 0f;
            float offset = 0f;
            bool found = false;
            foreach (var h in Physics.RaycastAll(pos + Vector3.up * 0.6f, Vector3.down, 1.2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (h.collider.transform.IsChildOf(student)) continue;          // his own controller
                float o = h.point.y - pos.y;
                if (!found || o > offset) { offset = o; found = true; }        // the highest surface under him (the rug over the floor)
            }
            if (!found) return groundKnown ? groundOffset : 0f;
            offset = Mathf.Clamp(offset, -0.25f, 0.1f) - footSink;
            groundOffset = groundKnown ? Mathf.Lerp(groundOffset, offset, 1f - Mathf.Exp(-20f * dt)) : offset;
            groundKnown = true;
            return groundOffset;
        }

        bool AtReadingSpot(Vector3 pos) =>
            mover != null && mover.AtGameplaySpot && mover.gameplaySpot != null &&
            Vector3.ProjectOnPlane(pos - mover.gameplaySpot.position, Vector3.up).magnitude < 0.6f;

        /// <summary>Idle-front: breathe slowly back and forth on the open-eye frames; blink only every few seconds.</summary>
        void IdleFront(out int a, out int b, out float t)
        {
            var blink = set.idleFrontBlinkFrames ?? new int[0];
            int n = set.idleFront.frames.Length;
            if (blink.Length > 0 && Time.time >= nextBlink && Time.time >= blinkUntil)
            {
                blinkUntil = Time.time + 0.32f;
                ScheduleBlink();
            }
            if (Time.time < blinkUntil)
            {
                float k = 1f - (blinkUntil - Time.time) / 0.32f;             // 0..1 through the blink
                int i = Mathf.Clamp(Mathf.FloorToInt(k * blink.Length * 2f), 0, blink.Length * 2 - 1);
                int idx = i < blink.Length ? i : blink.Length * 2 - 1 - i;     // close, then open again
                a = b = blink[idx]; t = 0f;
                return;
            }
            // ping-pong across the open frames so breathing never jumps from the last pose back to the first
            var open = new System.Collections.Generic.List<int>(n);
            for (int j = 0; j < n; j++) if (System.Array.IndexOf(blink, j) < 0) open.Add(j);
            int m = open.Count;
            float p = Mathf.PingPong(phase, Mathf.Max(1, m - 1));
            int i0 = Mathf.Clamp(Mathf.FloorToInt(p), 0, m - 1), i1 = Mathf.Min(i0 + 1, m - 1);
            a = open[i0]; b = open[i1]; t = p - i0;
        }

        void ScheduleBlink() => nextBlink = Time.time + Random.Range(blinkEvery.x, blinkEvery.y);

        /// <summary>Smile (front view) for a while — e.g. after a correct answer. Fades in and out.</summary>
        public void Smile(float seconds = 2f) => smileUntil = Mathf.Max(smileUntil, Time.time + seconds);

        public bool IsSmiling => smileWeight > 0.5f;

        float smileUntil, smileWeight, frontSince = -1f, nextAutoSmile;

        static Sprite Matching(StudentSpriteSet.Clip c, int idleFrame, int idleCount) =>
            c != null && c.frames != null && c.frames.Length > 0 ? c.frames[Mathf.Clamp(idleFrame * c.frames.Length / idleCount, 0, c.frames.Length - 1)] : null;

        /// <summary>
        /// Front idle rendered from the 3D model: blinks every few seconds and, after he has stood
        /// facing you a moment (or when asked), breaks into a grin that cross-fades in and out.
        /// The expression frames match the idle pose, so only the face changes.
        /// </summary>
        void FrontFace(int a, int n, bool flip)
        {
            if (frontSince < 0f) { frontSince = Time.time; nextAutoSmile = Time.time + smileAfter; }
            if (Time.time >= nextAutoSmile) { Smile(smileHold); nextAutoSmile = Time.time + Random.Range(smileEvery.x, smileEvery.y); }

            float target = Time.time < smileUntil ? 1f : 0f;
            smileWeight = Mathf.MoveTowards(smileWeight, target, Time.deltaTime / Mathf.Max(0.01f, smileFade));
            float w = Mathf.SmoothStep(0f, 1f, smileWeight);

            // Blink: half, closed, closed, half — only with a neutral face.
            if (w <= 0.01f && Time.time >= nextBlink && Time.time >= blinkUntil) { blinkUntil = Time.time + blinkTime; ScheduleBlink(); }
            Sprite face = null;
            if (Time.time < blinkUntil)
            {
                int step = Mathf.Clamp(Mathf.FloorToInt((1f - (blinkUntil - Time.time) / blinkTime) * 4f), 0, 3);
                face = Matching(step == 0 || step == 3 ? set.blinkHalf : set.blinkClosed, a, n);
            }
            if (face != null) spriteRenderer.sprite = face;

            var smile = Matching(set.smileFront, a, n);
            if (blendRenderer != null)
            {
                blendRenderer.sprite = smile;
                blendRenderer.flipX = flip;
                blendRenderer.color = new Color(1f, 1f, 1f, w);
                blendRenderer.enabled = use2D && smile != null && w > 0.01f;
            }
        }
    }
}
