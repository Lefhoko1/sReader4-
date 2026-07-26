using UnityEngine;
using UnityEngine.UIElements;

namespace SReader.Showcase
{
    /// <summary>
    /// Shows a 3D model FULL SCREEN at runtime, on top of the app's UI Toolkit
    /// screens. A normal camera can't beat overlay UI, so instead the model
    /// camera renders into a RenderTexture, and this component paints that
    /// texture across a high-sorting-order UI Toolkit panel (assigned on the
    /// UIDocument). The RenderTexture tracks the real screen size, so on the
    /// Huawei Y5 (720x1520) it fills the device exactly.
    ///
    /// Lives on the same GameObject as a UIDocument whose PanelSettings has a
    /// higher sortingOrder than the app panels.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class FullscreenModelStage : MonoBehaviour
    {
        [Tooltip("Camera that renders ONLY the model. Its output is shown full screen.")]
        public Camera modelCamera;

        RenderTexture _rt;
        Image _surface;
        int _w, _h;

        void OnDisable() { Release(); }

        void Update()
        {
            if (_rt == null || _surface == null || Screen.width != _w || Screen.height != _h)
                Rebuild();
        }

        void Rebuild()
        {
            var doc = GetComponent<UIDocument>();
            var root = doc != null ? doc.rootVisualElement : null;
            if (root == null) return; // panel not ready yet — retry next frame

            Release();

            _w = Mathf.Max(1, Screen.width);
            _h = Mathf.Max(1, Screen.height);

            _rt = new RenderTexture(_w, _h, 24, RenderTextureFormat.Default) { name = "Trial1StageRT" };
            _rt.Create();

            if (modelCamera != null)
            {
                modelCamera.targetTexture = _rt;
                modelCamera.aspect = (float)_w / _h; // portrait aspect -> framer fits to width
            }

            root.Clear();
            root.pickingMode = PickingMode.Ignore;
            _surface = new Image { image = _rt, scaleMode = ScaleMode.ScaleAndCrop };
            _surface.pickingMode = PickingMode.Ignore; // display only — don't trap touches
            _surface.style.position = Position.Absolute;
            _surface.StretchToParentSize();
            root.Add(_surface);
        }

        void Release()
        {
            if (modelCamera != null && modelCamera.targetTexture == _rt)
                modelCamera.targetTexture = null;

            if (_surface != null) { _surface.RemoveFromHierarchy(); _surface = null; }

            if (_rt != null)
            {
                _rt.Release();
                if (Application.isPlaying) Destroy(_rt); else DestroyImmediate(_rt);
                _rt = null;
            }
        }
    }
}
