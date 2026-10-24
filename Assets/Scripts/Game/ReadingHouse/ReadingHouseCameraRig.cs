using System;
using System.Collections;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    public enum ReadingHouseView { Overview = 0, Gameplay = 1, TrayFocus = 2 }

    /// <summary>
    /// The three approved Reading House camera views. Each pose is a position
    /// empty + an aim empty exported from Blender (SR_CAMPOS_* / SR_CAMTGT_*), so
    /// the views survive the Blender→FBX→Unity axis conversion unchanged.
    ///
    /// The vertical FOV values were authored for the 1520 × 720 reference screen.
    /// On other aspect ratios the HORIZONTAL field is held constant, so the
    /// dashboard keeps filling the width on taller and wider phones alike.
    ///
    /// "Keep verticals": instead of tilting the camera down at the dashboard
    /// (which makes text on either side lean like italics), the camera stays
    /// level and the lens is shifted down — the architectural-photography fix.
    /// The framing is the same; only the keystone distortion goes away.
    /// </summary>
    public sealed class ReadingHouseCameraRig : MonoBehaviour
    {
        [Serializable]
        public sealed class Pose
        {
            public ReadingHouseView view;
            public Transform position;
            public Transform aim;
            [Tooltip("Vertical FOV at the 1520 × 720 reference aspect.")]
            public float referenceVerticalFov = 45f;
            [Tooltip("Level camera + lens shift, so vertical lines and text stay upright.")]
            public bool keepVerticals = true;
        }

        public const float ReferenceAspect = 1520f / 720f;

        public Camera viewCamera;
        public Pose[] poses = new Pose[3];
        public float transitionSeconds = 0.6f;

        public ReadingHouseView Current { get; private set; }
        public bool IsMoving => motion != null;
        public event Action<ReadingHouseView> Arrived;

        Coroutine motion;

        public void GoTo(ReadingHouseView view, bool immediate = false)
        {
            var pose = Find(view);
            if (pose == null || viewCamera == null) return;
            viewCamera.usePhysicalProperties = true;
            viewCamera.gateFit = Camera.GateFitMode.Vertical;   // fieldOfView stays the vertical FOV we set
            if (motion != null) { StopCoroutine(motion); motion = null; }
            Current = view;
            if (immediate || !Application.isPlaying)
            {
                Apply(pose, 1f, Snapshot());
                Arrived?.Invoke(view);
            }
            else motion = StartCoroutine(Move(pose));
        }

        /// <summary>Vertical FOV that keeps the reference HORIZONTAL field on the current screen.</summary>
        public float FovFor(Pose pose)
        {
            float aspect = viewCamera.aspect > 0.01f ? viewCamera.aspect : ReferenceAspect;
            float halfH = Mathf.Atan(Mathf.Tan(pose.referenceVerticalFov * 0.5f * Mathf.Deg2Rad) * ReferenceAspect);
            return 2f * Mathf.Atan(Mathf.Tan(halfH) / aspect) * Mathf.Rad2Deg;
        }

        Pose Find(ReadingHouseView view)
        {
            foreach (var p in poses) if (p != null && p.view == view) return p;
            return null;
        }

        (Vector3 pos, Quaternion rot, float fov, Vector2 shift) Snapshot()
            => (viewCamera.transform.position, viewCamera.transform.rotation, viewCamera.fieldOfView, viewCamera.lensShift);

        IEnumerator Move(Pose pose)
        {
            var from = Snapshot();
            for (float t = 0f; t < transitionSeconds; t += Time.unscaledDeltaTime)
            {
                Apply(pose, Mathf.SmoothStep(0f, 1f, t / transitionSeconds), from);
                yield return null;
            }
            Apply(pose, 1f, from);
            motion = null;
            Arrived?.Invoke(pose.view);
        }

        void Apply(Pose pose, float k, (Vector3 pos, Quaternion rot, float fov, Vector2 shift) from)
        {
            Vector3 dir = (pose.aim.position - pose.position.position).normalized;
            float fov = FovFor(pose);
            Quaternion rot;
            Vector2 shift = Vector2.zero;
            if (pose.keepVerticals)
            {
                var flat = Vector3.ProjectOnPlane(dir, Vector3.up);
                rot = Quaternion.LookRotation(flat.sqrMagnitude > 1e-6f ? flat.normalized : Vector3.forward, Vector3.up);
                // Reproduce the tilted view's exact top and bottom edges on a level image plane.
                float pitch = Mathf.Asin(Mathf.Clamp(dir.y, -1f, 1f));              // negative when looking down
                float half = fov * 0.5f * Mathf.Deg2Rad;
                float tTop = Mathf.Tan(pitch + half), tBottom = Mathf.Tan(pitch - half);
                float height = tTop - tBottom;
                fov = 2f * Mathf.Atan(height * 0.5f) * Mathf.Rad2Deg;
                shift.y = (tTop + tBottom) * 0.5f / height;
            }
            else rot = Quaternion.LookRotation(dir, Vector3.up);

            viewCamera.transform.SetPositionAndRotation(Vector3.Lerp(from.pos, pose.position.position, k), Quaternion.Slerp(from.rot, rot, k));
            viewCamera.fieldOfView = Mathf.Lerp(from.fov, fov, k);
            viewCamera.lensShift = Vector2.Lerp(from.shift, shift, k);
        }
    }
}
