using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace SReader.Game.Classroom
{
    public sealed class ClassroomCameraRig : MonoBehaviour
    {
        [Serializable] public class Pose { public string name; public Vector3 position, target; public float fieldOfView = 60; public bool roofVisible; }
        public Camera viewCamera;
        public GameObject roof, frontCutaway;
        public Pose[] poses;
        public int currentView;
        public event Action<int> Arrived;
        Coroutine motion;
        public bool IsMoving => motion != null;
        void Awake() { Application.targetFrameRate = 30; Application.runInBackground = true; Screen.orientation = ScreenOrientation.Portrait; GoTo(0, true); }
        public void GoTo(int index, bool immediate = false)
        {
            if (index < 0 || poses == null || index >= poses.Length || !viewCamera) return;
            if (motion != null) { StopCoroutine(motion); motion = null; }
            currentView = index; var pose = poses[index];
            if (roof) roof.SetActive(pose.roofVisible);
            if (frontCutaway) frontCutaway.SetActive(pose.roofVisible);
            if (immediate) { Apply(pose, 1, viewCamera.transform.position, viewCamera.transform.rotation, viewCamera.fieldOfView); Arrived?.Invoke(index); }
            else motion = StartCoroutine(Move(pose, index));
        }
        IEnumerator Move(Pose pose, int index)
        {
            Vector3 position = viewCamera.transform.position; Quaternion rotation = viewCamera.transform.rotation; float fov = viewCamera.fieldOfView;
            for (float elapsed = 0; elapsed < .55f; elapsed += Time.unscaledDeltaTime) { Apply(pose, Mathf.SmoothStep(0, 1, elapsed / .55f), position, rotation, fov); yield return null; }
            Apply(pose, 1, position, rotation, fov); motion = null; Arrived?.Invoke(index);
        }
        void Apply(Pose pose, float t, Vector3 start, Quaternion rotation, float fov)
        {
            viewCamera.orthographic = false;
            viewCamera.transform.SetPositionAndRotation(Vector3.Lerp(start, pose.position, t), Quaternion.Slerp(rotation, Quaternion.LookRotation(pose.target - pose.position), t));
            viewCamera.fieldOfView = Mathf.Lerp(fov, pose.fieldOfView, t);
        }
#if UNITY_EDITOR
        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null) return;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) GoTo(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) GoTo(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) GoTo(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) GoTo(3);
#endif
        }
#endif
    }
}
