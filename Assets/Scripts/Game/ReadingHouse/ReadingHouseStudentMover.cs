using System.Collections;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Moves the student between the approved overview spot and the approved
    /// gameplay spot beside the left end of the dashboard
    /// (SR_ANCHOR_STUDENT_OVERVIEW / SR_ANCHOR_STUDENT_GAMEPLAY). At the overview
    /// spot his hair crosses the stone tray from every gameplay camera, so he
    /// always steps aside before a gameplay view is used.
    ///
    /// The characters are not rigged yet, so this is a smooth glide and turn,
    /// not a walk cycle. Swap in an Animator later without changing callers.
    /// </summary>
    public sealed class ReadingHouseStudentMover : MonoBehaviour
    {
        public Transform student;
        public Transform overviewSpot;
        public Transform gameplaySpot;
        public float moveSeconds = 0.8f;

        public bool AtGameplaySpot { get; private set; }
        /// <summary>True while gliding him to a spot (the walker stays off meanwhile).</summary>
        public bool Moving => motion != null;
        Coroutine motion;

        public void ToOverview(bool immediate = false) => MoveTo(overviewSpot, false, immediate);
        public void ToGameplay(bool immediate = false) => MoveTo(gameplaySpot, true, immediate);

        void MoveTo(Transform spot, bool gameplay, bool immediate)
        {
            if (student == null || spot == null) return;
            AtGameplaySpot = gameplay;
            if (motion != null) { StopCoroutine(motion); motion = null; }
            if (immediate || !Application.isPlaying) { student.SetPositionAndRotation(spot.position, spot.rotation); return; }
            motion = StartCoroutine(Glide(spot));
        }

        IEnumerator Glide(Transform spot)
        {
            Vector3 p0 = student.position; Quaternion r0 = student.rotation;
            for (float t = 0f; t < moveSeconds; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0f, 1f, t / moveSeconds);
                student.SetPositionAndRotation(Vector3.Lerp(p0, spot.position, k), Quaternion.Slerp(r0, spot.rotation, k));
                yield return null;
            }
            student.SetPositionAndRotation(spot.position, spot.rotation);
            motion = null;
        }
    }
}
