using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The reading die on the dashboard's tray: one richly made die whose three visible faces carry the
    /// choices: words, letters or numbered pictures. Tapping a face picks it (<see cref="ChoiceAt"/>); the
    /// die rolls on to the next choices, wobbles "no" at a wrong one, hops for joy at the end of a sentence,
    /// and becomes more precious as key words are restored (wood → silver → gold → crystal → rainbow gem),
    /// sparkling during long streaks. Presentation only: the presenter decides what the faces say and what
    /// a pick means.
    /// </summary>
    public sealed class ReadingDie : MonoBehaviour
    {
        [Serializable]
        public sealed class Tier
        {
            public string name;
            public Material body, plate, trim, gem;
            public Color ink = new Color32(40, 28, 20, 255);
        }

        public Camera viewCamera;
        public Tier[] tiers = new Tier[0];
        public TMP_FontAsset font;
        [Tooltip("Unlit material for pictures on the faces (texture set per face).")]
        public Material pictureMaterial;
        public ParticleSystem sparkle;
        public float rollSeconds = 0.42f;

        /// <summary>One choice on a face: a word / letter, or a picture with its number.</summary>
        public struct Choice
        {
            public string text;
            public Texture picture;
            public int number;
            public static Choice Word(string t) => new Choice { text = t };
            public static Choice Picture(Texture tex, int n) => new Choice { picture = tex, number = n };
        }

        public int Tier_ { get; private set; }
        public bool Busy => motion != null;

        sealed class Face
        {
            public Vector3 dir;                    // outward, in the die's local space
            public Transform anchor;
            public TextMeshPro text, number;
            public MeshRenderer picture;
            public int choice = -1;
        }

        readonly List<Face> faces = new List<Face>();
        readonly List<Renderer> bodies = new List<Renderer>(), plates = new List<Renderer>(), trims = new List<Renderer>(), gems = new List<Renderer>();
        Coroutine motion;
        Vector3 restPosition;
        MaterialPropertyBlock block;
        int glowChoice = -1;

        void Awake()
        {
            restPosition = transform.localPosition;
            block = new MaterialPropertyBlock();
            foreach (var r in GetComponentsInChildren<Renderer>(true))
            {
                if (r.name.StartsWith("Body")) bodies.Add(r);
                else if (r.name.StartsWith("Plate_")) { plates.Add(r); AddFace(r); }
                else if (r.name.StartsWith("Trim_")) trims.Add(r);
                else if (r.name.StartsWith("Gem_")) gems.Add(r);
            }
            SetTier(0);
            if (sparkle != null) { var em = sparkle.emission; em.rateOverTime = 0f; }
        }

        void AddFace(Renderer plate)
        {
            var local = transform.InverseTransformPoint(plate.bounds.center);
            var dir = Snap(local.normalized);
            var anchor = new GameObject("Face " + plate.name).transform;
            anchor.SetParent(transform, false);
            anchor.localPosition = dir * (local.magnitude + 0.022f);
            anchor.localRotation = Quaternion.LookRotation(-dir, Mathf.Abs(dir.y) > 0.9f ? Vector3.forward : Vector3.up);

            var f = new Face { dir = dir, anchor = anchor };
            var tgo = new GameObject("Word");
            tgo.transform.SetParent(anchor, false);
            f.text = tgo.AddComponent<TextMeshPro>();
            if (font != null) f.text.font = font;
            f.text.alignment = TextAlignmentOptions.Center;
            f.text.enableAutoSizing = true;
            f.text.fontSizeMin = 0.6f; f.text.fontSizeMax = 4.2f;
            f.text.fontStyle = FontStyles.Bold;
            f.text.enableWordWrapping = true;
            f.text.rectTransform.sizeDelta = new Vector2(0.64f, 0.6f);
            f.text.text = "";

            var ngo = new GameObject("Number");
            ngo.transform.SetParent(anchor, false);
            ngo.transform.localPosition = new Vector3(-0.27f, 0.27f, -0.004f);
            f.number = ngo.AddComponent<TextMeshPro>();
            if (font != null) f.number.font = font;
            f.number.alignment = TextAlignmentOptions.Center;
            f.number.fontSize = 2.2f; f.number.fontStyle = FontStyles.Bold;
            f.number.rectTransform.sizeDelta = new Vector2(0.16f, 0.16f);
            f.number.text = "";

            var pic = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(pic.GetComponent<Collider>());
            pic.name = "Picture";
            pic.transform.SetParent(anchor, false);
            pic.transform.localPosition = new Vector3(0f, 0f, -0.002f);
            pic.transform.localScale = new Vector3(0.62f, 0.62f, 1f);
            f.picture = pic.GetComponent<MeshRenderer>();
            f.picture.sharedMaterial = pictureMaterial;
            f.picture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            f.picture.enabled = false;
            faces.Add(f);
        }

        static Vector3 Snap(Vector3 v)
        {
            var a = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
            if (a.x >= a.y && a.x >= a.z) return new Vector3(Mathf.Sign(v.x), 0, 0);
            if (a.y >= a.z) return new Vector3(0, Mathf.Sign(v.y), 0);
            return new Vector3(0, 0, Mathf.Sign(v.z));
        }

        // ── tiers and sparkle ──────────────────────────────────────────────

        public void SetTier(int tier)
        {
            if (tiers.Length == 0) return;
            Tier_ = Mathf.Clamp(tier, 0, tiers.Length - 1);
            var t = tiers[Tier_];
            void Put(List<Renderer> list, Material m) { if (m != null) foreach (var r in list) r.sharedMaterial = m; }
            Put(bodies, t.body); Put(plates, t.plate); Put(trims, t.trim); Put(gems, t.gem);
            foreach (var f in faces) { f.text.color = t.ink; f.number.color = t.ink; }
        }

        /// <summary>Streak sparkle: 0 = none, 1 = a few stars, 2 = a shower.</summary>
        public void SetSparkle(int level)
        {
            if (sparkle == null) return;
            var em = sparkle.emission;
            em.rateOverTime = level <= 0 ? 0f : level == 1 ? 6f : 18f;
            if (level > 0 && !sparkle.isPlaying) sparkle.Play();
        }

        // ── showing choices ────────────────────────────────────────────────

        /// <summary>The three faces the camera sees at a given orientation: top first, then left-front, right-front.</summary>
        List<Face> VisibleAt(Quaternion rotation)
        {
            var cam = viewCamera != null ? viewCamera.transform.position : transform.position + Vector3.back * 5f;
            var toCam = (cam - transform.position).normalized;
            var flatRight = Vector3.Cross(Vector3.up, -Vector3.ProjectOnPlane(toCam, Vector3.up)).normalized;
            Face top = null; float best = -2f;
            foreach (var f in faces) { float y = (rotation * f.dir).y; if (y > best) { best = y; top = f; } }
            var sides = new List<Face>();
            foreach (var f in faces) if (f != top && Mathf.Abs((rotation * f.dir).y) < 0.5f) sides.Add(f);
            sides.Sort((a, b) => Vector3.Dot(rotation * b.dir, toCam).CompareTo(Vector3.Dot(rotation * a.dir, toCam)));
            var front = sides.GetRange(0, Mathf.Min(2, sides.Count));
            front.Sort((a, b) => Vector3.Dot(rotation * a.dir, flatRight).CompareTo(Vector3.Dot(rotation * b.dir, flatRight)));
            var list = new List<Face> { top };
            list.AddRange(front);
            return list;
        }

        /// <summary>Put choices on the faces that will be visible at <paramref name="rotation"/> (up to three), upright for the camera.</summary>
        void Assign(IReadOnlyList<Choice> choices, Quaternion rotation)
        {
            foreach (var f in faces) { f.choice = -1; f.text.text = ""; f.number.text = ""; f.picture.enabled = false; }
            var vis = VisibleAt(rotation);
            var camPos = viewCamera != null ? viewCamera.transform.position : transform.position + Vector3.back * 5f;
            var toCam = Vector3.ProjectOnPlane(camPos - transform.position, Vector3.up).normalized;
            for (int i = 0; i < vis.Count && i < choices.Count; i++)
            {
                var f = vis[i];
                var c = choices[i];
                f.choice = i;
                // Upright for the reader: sides read with world-up; the top reads "away from the camera" as up.
                var nWorld = rotation * f.dir;
                var upWorld = Mathf.Abs(nWorld.y) > 0.9f ? -toCam : Vector3.up;
                var worldRot = Quaternion.LookRotation(-nWorld, upWorld);
                f.anchor.localRotation = Quaternion.Inverse(rotation) * worldRot;
                if (c.picture != null)
                {
                    f.picture.enabled = true;
                    f.picture.GetPropertyBlock(block);
                    block.SetTexture("_BaseMap", c.picture);
                    block.SetTexture("_MainTex", c.picture);
                    f.picture.SetPropertyBlock(block);
                    f.number.text = c.number > 0 ? c.number.ToString() : "";
                }
                else f.text.text = c.text ?? "";
            }
            ApplyGlow();
        }

        /// <summary>Show choices now (no roll).</summary>
        public void Show(IReadOnlyList<Choice> choices) => Assign(choices, transform.localRotation);

        /// <summary>Roll a quarter turn towards the camera (or sideways for a flip) and show new choices.</summary>
        public void Roll(IReadOnlyList<Choice> choices, bool sideways = false, int direction = 1)
        {
            if (motion != null) { StopCoroutine(motion); Settle(); }
            var axis = sideways ? NearestAxis(Vector3.up) : NearestAxis(CameraRight());
            var target = Quaternion.AngleAxis(90f * direction, axis) * transform.localRotation;
            Assign(choices, target);
            motion = StartCoroutine(RollTo(target));
        }

        Vector3 CameraRight()
        {
            var cam = viewCamera != null ? viewCamera.transform : null;
            return cam != null ? Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized : Vector3.right;
        }

        /// <summary>The die's own axis (in its parent's space) closest to a direction: rolls stay on the die's lattice.</summary>
        Vector3 NearestAxis(Vector3 worldDir)
        {
            var parent = transform.parent;
            var d = parent != null ? parent.InverseTransformDirection(worldDir) : worldDir;
            Vector3 best = Vector3.up; float bestDot = -2f;
            foreach (var a in new[] { Vector3.right, Vector3.up, Vector3.forward })
            {
                var w = transform.localRotation * a;
                float dot = Mathf.Abs(Vector3.Dot(w, d));
                if (dot > bestDot) { bestDot = dot; best = Vector3.Dot(w, d) >= 0 ? w : -w; }
            }
            return best;
        }

        IEnumerator RollTo(Quaternion target)
        {
            var from = transform.localRotation;
            for (float t = 0f; t < rollSeconds; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0f, 1f, t / rollSeconds);
                transform.localRotation = Quaternion.Slerp(from, target, k);
                transform.localPosition = restPosition + Vector3.up * (Mathf.Sin(k * Mathf.PI) * 0.22f);
                yield return null;
            }
            transform.localRotation = target;
            transform.localPosition = restPosition;
            motion = null;
        }

        void Settle()
        {
            transform.localPosition = restPosition;
            motion = null;
        }

        /// <summary>A wrong pick: the die shakes its head.</summary>
        public void Shake()
        {
            if (motion != null) return;
            motion = StartCoroutine(Wobble());
        }

        IEnumerator Wobble()
        {
            var rest = transform.localRotation;
            var up = NearestAxis(Vector3.up);
            for (float t = 0f; t < 0.45f; t += Time.deltaTime)
            {
                float a = 11f * Mathf.Exp(-6f * t) * Mathf.Sin(t * 42f);
                transform.localRotation = Quaternion.AngleAxis(a, up) * rest;
                yield return null;
            }
            transform.localRotation = rest;
            motion = null;
        }

        /// <summary>A sentence or a key word done: a joyful hop with a full spin.</summary>
        public void Celebrate()
        {
            if (motion != null) { StopCoroutine(motion); Settle(); }
            motion = StartCoroutine(Hop());
        }

        IEnumerator Hop()
        {
            var rest = transform.localRotation;
            var up = NearestAxis(Vector3.up);
            const float seconds = 0.8f;
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                float k = t / seconds;
                transform.localRotation = Quaternion.AngleAxis(360f * Mathf.SmoothStep(0f, 1f, k), up) * rest;
                transform.localPosition = restPosition + Vector3.up * (Mathf.Sin(k * Mathf.PI) * 0.5f);
                yield return null;
            }
            transform.localRotation = rest;
            transform.localPosition = restPosition;
            motion = null;
        }

        /// <summary>Guide me: the face carrying this choice glows (−1 = none).</summary>
        public void Glow(int choice)
        {
            glowChoice = choice;
            ApplyGlow();
        }

        void ApplyGlow()
        {
            foreach (var f in faces)
            {
                var plate = plates.Find(p => Snap(transform.InverseTransformPoint(p.bounds.center).normalized) == f.dir);
                if (plate == null) continue;
                plate.GetPropertyBlock(block);
                block.SetColor("_EmissionColor", f.choice >= 0 && f.choice == glowChoice ? new Color(1f, 0.75f, 0.3f) * 1.2f : Color.black);
                plate.SetPropertyBlock(block);
            }
        }

        /// <summary>Which choice a tap landed on, from the hit point (−1: not a choice face, or mid-roll).</summary>
        public int ChoiceAt(Vector3 worldPoint)
        {
            if (motion != null) return -1;
            var local = transform.InverseTransformPoint(worldPoint);
            var dir = Snap(local);
            foreach (var f in faces) if (f.dir == dir) return f.choice;
            return -1;
        }
    }
}
