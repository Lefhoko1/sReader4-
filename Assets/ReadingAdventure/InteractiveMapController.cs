using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.Showcase
{
    [System.Serializable] public class NodeTappedEvent : UnityEvent<string> { }

    /// <summary>
    /// Touch/mouse controller for an interactive 3D world-map:
    ///   • one-finger drag / left-drag  -> orbit
    ///   • two-finger pinch / mouse wheel -> zoom
    ///   • quick tap / click (no drag)   -> pick a level node (raycast)
    ///
    /// Renders straight to the screen (no RenderTexture), so taps hit the 3D
    /// world directly via Physics raycasting — colliders are added to the model
    /// by the build command. No uGUI Canvas required. Works with either the new
    /// Input System or the legacy Input Manager.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class InteractiveMapController : MonoBehaviour
    {
        [Header("Target")]
        public Transform model;
        [Tooltip("Child names (contains) skipped when sizing the map, e.g. the giant ocean plane.")]
        public string[] ignoreInFit = { "Ocean", "Water", "Sea" };
        [Tooltip("Child names (contains) treated as tappable level nodes.")]
        public string[] nodeNameTokens = { "Node" };

        [Header("Start view")]
        public float yaw = 20f;
        public float pitch = 40f;
        [Range(0.3f, 1.2f)] public float startFill = 0.9f;

        [Header("Limits")]
        public float minPitch = 12f;
        public float maxPitch = 82f;

        [Header("Speeds")]
        public float orbitSpeed = 0.2f;   // degrees per pixel dragged
        public float zoomSpeed = 1f;

        [Header("Events")]
        public NodeTappedEvent onNodeTapped;

        Camera _cam;
        Vector3 _pivot;
        float _distance, _minDistance, _maxDistance, _radius = 1f;
        bool _logged;

        bool _down, _dragging;
        Vector2 _downPos, _lastPos;
        float _downTime, _lastPinch = -1f;
        const float DragThresholdPx = 12f;

        void Start()
        {
            _cam = GetComponent<Camera>();
            Recompute();
        }

        /// <summary>Re-measures the model and frames it as the starting view.</summary>
        public void Recompute()
        {
            if (_cam == null) _cam = GetComponent<Camera>();

            _logged = false;
            if (model != null && TryGetBounds(model, ignoreInFit, out var b))
            {
                _pivot = b.center;
                _radius = Mathf.Max(0.01f, b.extents.magnitude);
                float vHalf = _cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
                float aspect = _cam.aspect <= 0f ? 0.5f : _cam.aspect;
                float hHalf = Mathf.Atan(Mathf.Tan(vHalf) * aspect);
                float fit = _radius / Mathf.Sin(Mathf.Max(0.05f, Mathf.Min(vHalf, hHalf)));
                _distance = fit * startFill;
                _minDistance = fit * 0.25f;
                _maxDistance = fit * 1.6f;
                Debug.Log($"[InteractiveMap] bounds={b.size} center={b.center} radius={_radius:F2} " +
                          $"dist={_distance:F2} aspect={aspect:F3}");
            }
            else
            {
                _pivot = model != null ? model.position : Vector3.zero;
                _radius = 5f;
                _distance = 10f; _minDistance = 2f; _maxDistance = 40f;
                Debug.LogWarning("[InteractiveMap] No renderers found on the model — nothing to frame. " +
                                 "Is 'model' assigned and does trial3 contain meshes?");
            }
            Apply();
        }

        void Apply()
        {
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            var rot = Quaternion.Euler(pitch, yaw, 0f);
            transform.rotation = rot;
            transform.position = _pivot - rot * Vector3.forward * _distance;

            // Keep the model inside the view frustum at any scale / zoom level.
            if (_cam != null)
            {
                _cam.nearClipPlane = Mathf.Max(0.01f, _distance - _radius * 2f);
                _cam.farClipPlane = _distance + _radius * 4f + 10f;
            }
        }

        void Update()
        {
            // --- pinch zoom (two pointers) -> suppresses orbit/tap ---
            if (TouchCount() >= 2)
            {
                float d = Vector2.Distance(TouchPos(0), TouchPos(1));
                if (_lastPinch > 0f) Zoom((d - _lastPinch) * 0.01f * _distance * zoomSpeed);
                _lastPinch = d;
                _down = false; _dragging = true;
                return;
            }
            _lastPinch = -1f;

            // --- wheel zoom (editor) ---
            float scroll = ScrollDelta();
            if (Mathf.Abs(scroll) > 0.001f) Zoom(scroll * 0.1f * _distance * zoomSpeed);

            // --- orbit / tap ---
            var pos = PointerPos();
            if (PointerDown())
            {
                _down = true; _dragging = false;
                _downPos = _lastPos = pos; _downTime = Time.time;
            }
            else if (_down && PointerHeld())
            {
                if ((pos - _downPos).magnitude > DragThresholdPx) _dragging = true;
                if (_dragging)
                {
                    var delta = pos - _lastPos;
                    yaw += delta.x * orbitSpeed;
                    pitch -= delta.y * orbitSpeed;
                    Apply();
                }
                _lastPos = pos;
            }
            else if (_down && PointerUp())
            {
                if (!_dragging && Time.time - _downTime < 0.4f) Pick(pos);
                _down = false;
            }
        }

        void Zoom(float closer)
        {
            _distance -= closer;
            Apply();
        }

        void Pick(Vector2 screenPos)
        {
            var ray = _cam.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, 10000f)) return;

            string node = FindNodeName(hit.collider.transform);
            if (node != null)
            {
                Debug.Log($"[Map] Tapped node: {node}");
                onNodeTapped?.Invoke(node);
            }
            else
            {
                Debug.Log($"[Map] Tapped: {hit.collider.name}");
            }
        }

        string FindNodeName(Transform t)
        {
            for (var cur = t; cur != null; cur = cur.parent)
                foreach (var token in nodeNameTokens)
                    if (!string.IsNullOrEmpty(token) &&
                        cur.name.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return cur.name;
            return null;
        }

        // ---- bounds (ignores oversized background pieces like the ocean) ----
        static bool TryGetBounds(Transform root, string[] ignore, out Bounds bounds)
        {
            bounds = new Bounds(root.position, Vector3.zero);
            var renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0) return false;

            bool has = false;
            foreach (var r in renderers)
            {
                if (IsIgnored(r.transform, root, ignore)) continue;
                if (!has) { bounds = r.bounds; has = true; } else bounds.Encapsulate(r.bounds);
            }
            if (!has)
            {
                bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
                has = true;
            }
            return has;
        }

        static bool IsIgnored(Transform t, Transform root, string[] ignore)
        {
            if (ignore == null || ignore.Length == 0) return false;
            for (var cur = t; cur != null && cur != root.parent; cur = cur.parent)
                foreach (var token in ignore)
                    if (!string.IsNullOrEmpty(token) &&
                        cur.name.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
            return false;
        }

        // ---- input abstraction (new Input System or legacy) ----
#if ENABLE_INPUT_SYSTEM
        int TouchCount()
        {
            var ts = Touchscreen.current;
            if (ts == null) return 0;
            int c = 0;
            foreach (var t in ts.touches) if (t.press.isPressed) c++;
            return c;
        }
        Vector2 TouchPos(int index)
        {
            var ts = Touchscreen.current;
            if (ts == null) return default;
            int c = 0;
            foreach (var t in ts.touches)
                if (t.press.isPressed) { if (c == index) return t.position.ReadValue(); c++; }
            return default;
        }
        bool PointerDown()
        {
            var m = Mouse.current; if (m != null && m.leftButton.wasPressedThisFrame) return true;
            var ts = Touchscreen.current; return ts != null && ts.primaryTouch.press.wasPressedThisFrame;
        }
        bool PointerHeld()
        {
            var m = Mouse.current; if (m != null && m.leftButton.isPressed) return true;
            var ts = Touchscreen.current; return ts != null && ts.primaryTouch.press.isPressed;
        }
        bool PointerUp()
        {
            var m = Mouse.current; if (m != null && m.leftButton.wasReleasedThisFrame) return true;
            var ts = Touchscreen.current; return ts != null && ts.primaryTouch.press.wasReleasedThisFrame;
        }
        Vector2 PointerPos()
        {
            var ts = Touchscreen.current;
            if (ts != null && ts.primaryTouch.press.isPressed) return ts.primaryTouch.position.ReadValue();
            var m = Mouse.current; return m != null ? m.position.ReadValue() : default;
        }
        float ScrollDelta()
        {
            var m = Mouse.current; return m != null ? m.scroll.ReadValue().y / 120f : 0f;
        }
#else
        int TouchCount() => Input.touchCount;
        Vector2 TouchPos(int index) => Input.GetTouch(index).position;
        bool PointerDown() => Input.GetMouseButtonDown(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool PointerHeld() => Input.GetMouseButton(0) || Input.touchCount > 0;
        bool PointerUp() => Input.GetMouseButtonUp(0) ||
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
        Vector2 PointerPos() => Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        float ScrollDelta() => Input.mouseScrollDelta.y;
#endif
    }
}
