using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// A 2D character in one spot of the house (the Encourager and the Boredom Zombie on their chairs).
    /// Presentation only: something asks for a named action (Play / PlayFor) and it plays the frames,
    /// then settles back into the breathing idle, which blinks now and then and, if set, breaks into an
    /// idle quirk every so often (the zombie's yawn, or getting up with a sigh).
    ///
    ///   • The sprite plane turns about the vertical axis to face the camera (yaw only).
    ///   • With a face target (the student) and several facings, the character turns to look at him —
    ///     the facing switches mid-action, keeping its place in the clip, and cross-fades.
    ///   • Every change of clip cross-fades through a second renderer, so nothing pops.
    ///   • The original 3D character is hidden, not deleted.
    /// </summary>
    public sealed class ActorSpriteAnimator : MonoBehaviour
    {
        [Header("Wiring")]
        public Camera viewCamera;
        public ActorSpriteSet set;
        public SpriteRenderer spriteRenderer;
        [Tooltip("Second renderer used for cross-fades (created automatically if empty).")]
        public SpriteRenderer blendRenderer;
        public Transform shadow;
        [Tooltip("The original 3D character, hidden while the 2D one is shown.")]
        public Transform model3D;
        [Tooltip("Optional: who to turn towards (needs several facings in the set).")]
        public Transform faceTarget;

        [Header("Feel")]
        public float crossFade = 0.18f;
        public Vector2 blinkEvery = new Vector2(3f, 6f);
        public float blinkTime = 0.25f;
        [Tooltip("Actions played on their own every so often while idle (one picked at random).")]
        public string[] idleQuirks = new string[0];
        public Vector2 idleQuirkEvery = new Vector2(9f, 16f);
        [Tooltip("Degrees past the half-way point between two facings before turning (no flicker).")]
        public float facingHysteresis = 6f;

        [Tooltip("Off = show the original 3D character instead.")]
        public bool use2D = true;

        public string Current { get; private set; } = "";
        public bool IsIdle => string.IsNullOrEmpty(Current);
        public float CurrentYaw => set != null && set.facings.Length > 0 ? set.facings[facing].yaw : 0f;

        int facing;
        StudentSpriteSet.Clip clip;
        float clipFps, phase, actionUntil, nextBlink, blinkUntil, nextQuirk;
        Sprite fadeFrom; float fadeT = 1f;
        Renderer[] old3D;

        ActorSpriteSet.Facing F => set.facings[facing];

        /// <summary>Play a named action once (non-looping clips) or for one loop.</summary>
        public void Play(string actionName)
        {
            var a = Usable(actionName);
            if (a != null) Begin(a, a.clip.frames.Length / Mathf.Max(1f, a.fps));
        }

        /// <summary>Play a (looping) named action for a while, e.g. talking while a speech bubble shows.</summary>
        public void PlayFor(string actionName, float seconds)
        {
            var a = Usable(actionName);
            if (a != null) Begin(a, seconds);
        }

        ActorSpriteSet.Action Usable(string actionName)
        {
            if (set == null || set.facings.Length == 0) return null;
            var a = F.Find(actionName);
            return a?.clip?.frames != null && a.clip.frames.Length > 0 ? a : null;
        }

        void Begin(ActorSpriteSet.Action a, float seconds)
        {
            Current = a.name;
            actionUntil = Time.time + seconds;
            Switch(a.clip, a.fps, keepPhase: false);
            nextQuirk = Time.time + seconds + Random.Range(idleQuirkEvery.x, idleQuirkEvery.y);
        }

        void Switch(StudentSpriteSet.Clip next, float fps, bool keepPhase)
        {
            if (next == null || next.frames == null || next.frames.Length == 0) { next = F.idle; fps = set.idleFps; }
            fadeFrom = spriteRenderer != null ? spriteRenderer.sprite : null;
            fadeT = 0f;
            float k = clip != null && clip.frames.Length > 0 ? phase / clip.frames.Length : 0f;
            clip = next; clipFps = fps;
            phase = keepPhase ? k * next.frames.Length : 0f;
        }

        void OnEnable()
        {
            EnsureBlendRenderer();
            old3D = model3D != null ? model3D.GetComponentsInChildren<Renderer>(true) : new Renderer[0];
            foreach (var r in old3D) if (r != null) r.enabled = !use2D;
            if (spriteRenderer != null) spriteRenderer.enabled = use2D;
            if (shadow != null) shadow.gameObject.SetActive(use2D);
            nextBlink = Time.time + Random.Range(blinkEvery.x, blinkEvery.y);
            nextQuirk = Time.time + Random.Range(idleQuirkEvery.x, idleQuirkEvery.y);
            if (set != null && set.facings.Length > 0)
            {
                facing = set.Nearest(0f);
                clip = F.idle; clipFps = set.idleFps; Current = "";
            }
        }

        void OnDisable()
        {
            if (old3D != null) foreach (var r in old3D) if (r != null) r.enabled = true;
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

        /// <summary>The yaw that would face the target: + when it is to the camera's right of the character.</summary>
        float YawTowardsTarget()
        {
            var toTarget = Vector3.ProjectOnPlane(faceTarget.position - transform.position, Vector3.up);
            var toCamera = Vector3.ProjectOnPlane(viewCamera.transform.position - transform.position, Vector3.up);
            if (toTarget.sqrMagnitude < 1e-4f || toCamera.sqrMagnitude < 1e-4f) return 0f;
            var right = Vector3.ProjectOnPlane(viewCamera.transform.right, Vector3.up).normalized;
            return Mathf.Atan2(Vector3.Dot(toTarget, right), Vector3.Dot(toTarget, toCamera.normalized)) * Mathf.Rad2Deg;
        }

        void UpdateFacing()
        {
            if (faceTarget == null || viewCamera == null || set.facings.Length < 2) return;
            float want = YawTowardsTarget();
            int best = set.Nearest(want);
            if (best == facing) return;
            // only turn once clearly past the half-way point between the two facings
            float half = Mathf.Abs(set.facings[best].yaw - F.yaw) * 0.5f;
            if (Mathf.Abs(want - F.yaw) < half + facingHysteresis) return;
            facing = best;
            var a = IsIdle ? null : F.Find(Current);
            if (a != null) Switch(a.clip, a.fps, keepPhase: true);
            else Switch(F.idle, set.idleFps, keepPhase: true);
        }

        void LateUpdate()
        {
            if (set == null || set.facings.Length == 0 || spriteRenderer == null || clip == null) return;

            if (viewCamera != null)
            {
                var f = Vector3.ProjectOnPlane(transform.position - viewCamera.transform.position, Vector3.up);
                if (f.sqrMagnitude > 1e-6f) transform.rotation = Quaternion.LookRotation(f.normalized, Vector3.up);
            }
            if (shadow != null) shadow.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);

            UpdateFacing();

            // Finished action -> idle. An idle quirk now and then.
            if (!IsIdle && Time.time >= actionUntil) { Current = ""; Switch(F.idle, set.idleFps, keepPhase: false); }
            if (IsIdle && idleQuirks.Length > 0 && Time.time >= nextQuirk) Play(idleQuirks[Random.Range(0, idleQuirks.Length)]);

            int n = clip.frames.Length;
            phase += Time.deltaTime * clipFps;
            int a;
            if (clip.loop) { phase = Mathf.Repeat(phase, n); a = Mathf.FloorToInt(phase) % n; }
            else a = Mathf.Min(Mathf.FloorToInt(phase), n - 1);

            Sprite frame = clip.frames[a];
            if (IsIdle && clip == F.idle) frame = Blink(a, n) ?? frame;
            spriteRenderer.sprite = frame;

            fadeT = Mathf.MoveTowards(fadeT, 1f, Time.deltaTime / Mathf.Max(0.01f, crossFade));
            if (blendRenderer != null)
            {
                bool fading = fadeT < 1f && fadeFrom != null;
                blendRenderer.enabled = use2D && fading;
                if (fading) { blendRenderer.sprite = fadeFrom; blendRenderer.color = new Color(1f, 1f, 1f, 1f - Mathf.SmoothStep(0f, 1f, fadeT)); }
            }
        }

        /// <summary>Half, closed, closed, half (closed only if there are no half frames) — over the idle pose.</summary>
        Sprite Blink(int idleFrame, int idleCount)
        {
            if (Time.time >= nextBlink && Time.time >= blinkUntil)
            {
                blinkUntil = Time.time + blinkTime;
                nextBlink = Time.time + Random.Range(blinkEvery.x, blinkEvery.y);
            }
            if (Time.time >= blinkUntil) return null;
            int step = Mathf.Clamp(Mathf.FloorToInt((1f - (blinkUntil - Time.time) / blinkTime) * 4f), 0, 3);
            bool hasHalf = F.blinkHalf?.frames != null && F.blinkHalf.frames.Length > 0;
            var c = (step == 0 || step == 3) && hasHalf ? F.blinkHalf : F.blinkClosed;
            if (c?.frames == null || c.frames.Length == 0) return null;
            return c.frames[Mathf.Clamp(idleFrame * c.frames.Length / idleCount, 0, c.frames.Length - 1)];
        }
    }
}
