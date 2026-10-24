using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>Keeps a flat glow / halo turned towards the camera (e.g. the signpost lantern's soft light).</summary>
    public sealed class FaceCamera : MonoBehaviour
    {
        static Camera cam;

        void LateUpdate()
        {
            if (cam == null) { var rig = FindAnyObjectByType<ReadingHouseCameraRig>(); cam = rig != null ? rig.viewCamera : Camera.main; }
            if (cam == null) return;
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, cam.transform.up);
        }
    }
}
