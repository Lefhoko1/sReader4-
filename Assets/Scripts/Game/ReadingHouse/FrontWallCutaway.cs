using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Dollhouse cut-away: when the student is inside, the front wall (and the
    /// door, sign-in panels and their labels) fades out so the camera can stay at
    /// the same distance behind him; when he walks back out it fades in again.
    /// The meshes are never moved or scaled, and their colliders stay on, so the
    /// walls still block him. Fading uses transparent copies of the wall's few
    /// materials, only while a fade is running.
    /// </summary>
    public sealed class FrontWallCutaway : MonoBehaviour
    {
        public Renderer[] renderers;
        public TMP_Text[] labels;
        public Transform student;
        public Transform doorway;
        [Tooltip("Points out of the house, through the front wall.")]
        public Vector3 outward = Vector3.forward;
        public float hideWhenInsideBy = 0.6f;
        public float showWhenOutsideBy = 0.2f;
        public float fadeSeconds = 0.45f;
        [Tooltip("The camera: the wall only fades while it stands between the camera and the room (looking in from the front). Swing the view round to the side and it comes back.")]
        public Camera viewer;

        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        Material[][] originals;
        readonly Dictionary<Material, Material> fadeCopies = new Dictionary<Material, Material>();
        Color[] labelColors;
        float alpha = 1f;
        bool hidden;

        public bool Hidden => hidden;

        void Awake()
        {
            originals = new Material[renderers.Length][];
            for (int i = 0; i < renderers.Length; i++) originals[i] = renderers[i].sharedMaterials;
            labelColors = new Color[labels.Length];
            for (int i = 0; i < labels.Length; i++) labelColors[i] = labels[i].color;
        }

        void Update()
        {
            if (student == null || doorway == null) return;
            float side = Vector3.Dot(student.position - doorway.position, outward.normalized);
            if (!hidden && side < -hideWhenInsideBy && LooksInThroughWall()) hidden = true;
            else if (hidden && (side > -showWhenOutsideBy || !LooksInThroughWall())) hidden = false;

            float target = hidden ? 0f : 1f;
            if (Mathf.Approximately(alpha, target)) return;
            float before = alpha;
            alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime / fadeSeconds);
            Apply(before);
        }

        /// <summary>Is the camera out in front of the wall, looking in through it? (From the side it doesn't block anything.)</summary>
        bool LooksInThroughWall()
        {
            if (viewer == null) { var rig = FindAnyObjectByType<ReadingHouseCameraRig>(); viewer = rig != null ? rig.viewCamera : Camera.main; }
            if (viewer == null) return true;
            var o = outward.normalized;
            var cam = viewer.transform;
            bool inFront = Vector3.Dot(cam.position - doorway.position, o) > 0.3f;
            bool facingIn = Vector3.Dot(Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized, -o) > 0.5f;   // within ~60° of straight in
            return inFront && facingIn;
        }

        void Apply(float before)
        {
            float eased = Mathf.SmoothStep(0f, 1f, alpha);
            if (alpha >= 1f)
            {
                for (int i = 0; i < renderers.Length; i++) { renderers[i].enabled = true; renderers[i].sharedMaterials = originals[i]; }
            }
            else if (alpha <= 0f)
            {
                foreach (var r in renderers) r.enabled = false;
            }
            else
            {
                if (before >= 1f || before <= 0f)
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].enabled = true;
                        var mats = new Material[originals[i].Length];
                        for (int m = 0; m < mats.Length; m++) mats[m] = FadeCopy(originals[i][m]);
                        renderers[i].sharedMaterials = mats;
                    }
                foreach (var pair in fadeCopies)
                {
                    var c = pair.Key.HasProperty(BaseColor) ? pair.Key.GetColor(BaseColor) : Color.white;
                    c.a *= eased;
                    pair.Value.SetColor(BaseColor, c);
                }
            }
            for (int i = 0; i < labels.Length; i++)
            {
                var c = labelColors[i]; c.a *= eased; labels[i].color = c;
                labels[i].enabled = alpha > 0f;
            }
        }

        Material FadeCopy(Material original)
        {
            if (original == null) return null;
            if (fadeCopies.TryGetValue(original, out var copy)) return copy;
            copy = new Material(original) { name = original.name + " (fade)" };
            copy.SetFloat("_Surface", 1f);
            copy.SetFloat("_Blend", 0f);
            copy.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            copy.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            copy.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            copy.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
            copy.SetFloat("_ZWrite", 0f);
            copy.SetOverrideTag("RenderType", "Transparent");
            copy.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            copy.renderQueue = (int)RenderQueue.Transparent;
            fadeCopies[original] = copy;
            return copy;
        }

        void OnDestroy()
        {
            foreach (var c in fadeCopies.Values) if (c != null) Destroy(c);
        }
    }
}
