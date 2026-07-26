using UnityEngine;

namespace SReader.Showcase
{
    /// <summary>
    /// Keeps a target model fully framed on screen for ANY resolution / aspect,
    /// mirroring how the project's UI Toolkit PanelSettings use
    /// Scale-With-Screen-Size (reference 360x800, match width). On a tall
    /// portrait device like the Huawei Y5 2019 (720x1520, 19:9) the horizontal
    /// field is the limiting axis, so the model is framed to fit the width and
    /// stays fully visible — exactly like the UI scales.
    ///
    /// Runs in edit mode too ([ExecuteAlways]) so the framing updates live as you
    /// resize the Game view. Attach to a Camera and assign <see cref="target"/>.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class PortraitModelFramer : MonoBehaviour
    {
        [Tooltip("Root of the model to keep fully framed.")]
        public Transform target;

        [Tooltip("Camera view angle — higher X is more top-down, which fills a portrait screen better.")]
        public Vector3 viewEulerAngles = new Vector3(30f, 20f, 0f);

        [Tooltip("Breathing room around the model. 1 = edge-to-edge, 1.08 = 8% margin.")]
        public float padding = 1.0f;

        [Tooltip("Extra zoom on top of the auto-fit. >1 enlarges the model and crops the " +
                 "surrounding empty space — the way to fill a tall portrait screen with a flat, wide model.")]
        public float zoom = 1.5f;

        [Tooltip("Child objects whose name contains any of these words are IGNORED when measuring " +
                 "how big the model is (they still render). Use it to skip oversized background pieces " +
                 "like a giant ocean/water plane so framing fits the interesting content.")]
        public string[] ignoreInFit = { "Ocean", "Water", "Sea" };

        [Tooltip("Logs the computed framing once per enable (Console) to confirm this camera is the one being shown.")]
        public bool logFramingOnce = true;

        [Tooltip("Shifts the framed centre up/down as a fraction of the model height. " +
                 "Positive pushes the model lower on screen, leaving headroom for a top UI bar.")]
        public float verticalBias = 0f;

        Camera _cam;
        float _lastAspect = -1f;
        Vector3 _lastTargetPos;
        bool _dirty = true;
        bool _logged;

        void OnEnable() { _cam = GetComponent<Camera>(); _dirty = true; _logged = false; Frame(); }
        void OnValidate() { _dirty = true; }

        void Update()
        {
            if (target == null) return;
            if (_cam == null) _cam = GetComponent<Camera>();
            if (_dirty
                || !Mathf.Approximately(_cam.aspect, _lastAspect)
                || target.position != _lastTargetPos)
            {
                Frame();
            }
        }

        /// <summary>Re-positions the camera so the whole model fits the current aspect.</summary>
        public void Frame()
        {
            if (target == null) return;
            if (_cam == null) _cam = GetComponent<Camera>();
            if (!TryGetBounds(target, ignoreInFit, out var bounds)) return;

            transform.rotation = Quaternion.Euler(viewEulerAngles);
            Vector3 right = transform.right, up = transform.up, fwd = transform.forward;
            Vector3 center = bounds.center;

            float vHalf = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float aspect = _cam.aspect <= 0f ? 1f : _cam.aspect;
            float tanV = Mathf.Tan(vHalf);
            float tanH = tanV * aspect;

            // Tight fit: measure the model's actual silhouette as seen by the camera
            // by projecting the 8 AABB corners onto the camera's right/up/forward
            // axes. This fills the frame far better than a bounding sphere for a flat,
            // wide object — on portrait the width is the limiting axis ("match width").
            Vector3 e = bounds.extents;
            float maxX = 0f, maxY = 0f, maxZ = 0f;
            for (int i = 0; i < 8; i++)
            {
                Vector3 d = new Vector3(
                    (i & 1) == 0 ? -e.x : e.x,
                    (i & 2) == 0 ? -e.y : e.y,
                    (i & 4) == 0 ? -e.z : e.z);
                maxX = Mathf.Max(maxX, Mathf.Abs(Vector3.Dot(d, right)));
                maxY = Mathf.Max(maxY, Mathf.Abs(Vector3.Dot(d, up)));
                maxZ = Mathf.Max(maxZ, Mathf.Abs(Vector3.Dot(d, fwd)));
            }

            float pad = Mathf.Max(1f, padding);
            float distForWidth = (maxX * pad) / tanH + maxZ;
            float distForHeight = (maxY * pad) / tanV + maxZ;
            float dist = Mathf.Max(distForWidth, distForHeight) / Mathf.Max(0.05f, zoom);

            Vector3 framedCenter = center + up * (maxY * 2f * verticalBias);
            transform.position = framedCenter - fwd * dist;

            float span = Mathf.Max(maxX, Mathf.Max(maxY, maxZ));
            _cam.nearClipPlane = Mathf.Max(0.01f, dist - maxZ - span - 1f);
            _cam.farClipPlane = dist + maxZ + span + 10f;

            if (logFramingOnce && !_logged)
            {
                _logged = true;
                Debug.Log($"[PortraitModelFramer] target='{target.name}' boundsSize={bounds.size} " +
                          $"maxX={maxX:F2} maxY={maxY:F2} maxZ={maxZ:F2} aspect={aspect:F3} zoom={zoom} dist={dist:F2}");
            }

            _lastAspect = _cam.aspect;
            _lastTargetPos = target.position;
            _dirty = false;
        }

        static bool TryGetBounds(Transform root, string[] ignore, out Bounds bounds)
        {
            bounds = new Bounds(root.position, Vector3.zero);
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0) return false;

            // First pass: everything except the ignored (oversized background) pieces.
            bool has = false;
            foreach (var r in renderers)
            {
                if (IsIgnored(r.transform, root, ignore)) continue;
                if (!has) { bounds = r.bounds; has = true; }
                else bounds.Encapsulate(r.bounds);
            }

            // Fallback: if everything was ignored (or names didn't match), use all.
            if (!has)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);
                has = true;
            }
            return has;
        }

        static bool IsIgnored(Transform t, Transform root, string[] ignore)
        {
            if (ignore == null || ignore.Length == 0) return false;
            // Walk from the renderer up to the model root, matching any name token.
            for (var cur = t; cur != null && cur != root.parent; cur = cur.parent)
            {
                string n = cur.name;
                foreach (var token in ignore)
                    if (!string.IsNullOrEmpty(token) &&
                        n.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
            }
            return false;
        }
    }
}
