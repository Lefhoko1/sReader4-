using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SReader.EnvironmentPreview
{
    /// <summary>Presentation controls for the dedicated library preview scene.</summary>
    public sealed class LibraryRedesignPreview : MonoBehaviour
    {
        [Serializable]
        public class ViewPreset
        {
            public string label;
            public Vector3 position;
            public Quaternion rotation;
            public bool orthographic;
            public float orthographicSize;
            public float fieldOfView;
            public bool showRoof;
        }
        public Camera previewCamera;
        public GameObject roof;
        public ViewPreset[] views;
        public int currentView;
        const float ReferenceAspect = 4f / 3f;
        float lastAspect;
        void Awake() { ShowView(currentView); }
        public void ShowView(int index)
        {
            if (previewCamera == null || views == null || index < 0 || index >= views.Length) return;
            currentView = index;
            var view = views[index];
            previewCamera.transform.SetPositionAndRotation(view.position, view.rotation);
            previewCamera.orthographic = view.orthographic;
            if (roof != null) roof.SetActive(view.showRoof);
            FitAspect();
        }
        void FitAspect()
        {
            if (views == null || views.Length == 0 || previewCamera == null) return;
            var view = views[currentView];
            float aspect = Mathf.Max(.2f, previewCamera.aspect);
            previewCamera.orthographicSize = view.orthographicSize * Mathf.Max(1f, ReferenceAspect / aspect);
            float tangent = Mathf.Tan(view.fieldOfView * Mathf.Deg2Rad * .5f);
            previewCamera.fieldOfView = Mathf.Atan(tangent * Mathf.Max(1f, ReferenceAspect / aspect)) * 2f * Mathf.Rad2Deg;
            lastAspect = aspect;
        }
        void Update()
        {
            if (previewCamera != null && !Mathf.Approximately(previewCamera.aspect, lastAspect)) FitAspect();
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.digit1Key.wasPressedThisFrame) ShowView(0);
                if (keyboard.digit2Key.wasPressedThisFrame) ShowView(1);
                if (keyboard.digit3Key.wasPressedThisFrame) ShowView(2);
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Alpha1)) ShowView(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ShowView(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ShowView(2);
#endif
        }
        void OnGUI()
        {
            if (views == null) return;
            float scale = Mathf.Clamp(Screen.width / 1200f, .65f, 1.4f);
            var old = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * scale);
            float y = Screen.height / scale - 60;
            GUI.Box(new Rect(14, y - 6, views.Length * 114 + 12, 52), GUIContent.none);
            for (int i = 0; i < views.Length; i++)
            {
                GUI.backgroundColor = i == currentView ? new Color(.84f, .68f, .35f) : new Color(.35f, .38f, .44f);
                if (GUI.Button(new Rect(20 + i * 114, y, 108, 40), views[i].label)) ShowView(i);
            }
            GUI.backgroundColor = Color.white;
            GUI.matrix = old;
        }
    }
}
