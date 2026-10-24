using System.Collections;
using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The Reading House front doors. They stay shut — with a blocker in the
    /// doorway so the student cannot walk through — until <see cref="Unlock"/> is
    /// called after a successful sign-in. Once unlocked they swing open into the
    /// house as soon as the student comes near. Hinges are the Blender empties
    /// SR_Entry_Hinge_-1 / SR_Entry_Hinge_1 (exported closed).
    /// </summary>
    public sealed class HouseDoor : MonoBehaviour
    {
        public Transform leftHinge, rightHinge;
        public Transform student;
        public BoxCollider doorwayBlocker;
        [Tooltip("Where 'inside' is, so the doors swing into the house.")]
        public Transform houseCentre;
        public float openAngle = 95f;
        public float openSeconds = 1.1f;
        public float openWhenWithin = 3.2f;

        public bool Unlocked { get; private set; }
        public bool IsOpen { get; private set; }

        Quaternion leftClosed, rightClosed;
        float leftSign = 1f, rightSign = 1f;

        void Awake()
        {
            leftClosed = leftHinge.localRotation;
            rightClosed = rightHinge.localRotation;
            leftSign = InwardSign(leftHinge);
            rightSign = InwardSign(rightHinge);
        }

        /// <summary>Which way round the world up axis turns this leaf INTO the house.</summary>
        float InwardSign(Transform hinge)
        {
            var leaf = hinge.GetComponentInChildren<Renderer>();
            if (leaf == null || houseCentre == null) return 1f;
            Vector3 arm = Vector3.ProjectOnPlane(leaf.bounds.center - hinge.position, Vector3.up);
            Vector3 swung = Quaternion.AngleAxis(30f, Vector3.up) * arm;
            Vector3 inward = Vector3.ProjectOnPlane(houseCentre.position - hinge.position, Vector3.up);
            return Vector3.Dot(swung - arm, inward) > 0f ? 1f : -1f;
        }

        public float DistanceToStudent => student != null
            ? Vector3.Distance(Vector3.ProjectOnPlane(student.position, Vector3.up), Vector3.ProjectOnPlane(transform.position, Vector3.up))
            : float.MaxValue;

        /// <summary>Signed in: the doors may open (they do when the student is near).</summary>
        public void Unlock() => Unlocked = true;

        /// <summary>Signed out: lock, and swing the doors shut if they are open.</summary>
        public void Lock()
        {
            Unlocked = false;
            if (!IsOpen) return;
            StopAllCoroutines();
            StartCoroutine(Shut());
        }

        IEnumerator Shut()
        {
            var l0 = leftHinge.localRotation; var r0 = rightHinge.localRotation;
            for (float t = 0f; t < openSeconds; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0f, 1f, t / openSeconds);
                leftHinge.localRotation = Quaternion.Slerp(l0, leftClosed, k);
                rightHinge.localRotation = Quaternion.Slerp(r0, rightClosed, k);
                yield return null;
            }
            leftHinge.localRotation = leftClosed;
            rightHinge.localRotation = rightClosed;
            if (doorwayBlocker != null) doorwayBlocker.enabled = true;
            IsOpen = false;
        }

        void Update()
        {
            if (Unlocked && !IsOpen && DistanceToStudent <= openWhenWithin) StartCoroutine(Open());
        }

        IEnumerator Open()
        {
            IsOpen = true;
            var lTarget = Quaternion.AngleAxis(openAngle * leftSign, Vector3.up);
            var rTarget = Quaternion.AngleAxis(openAngle * rightSign, Vector3.up);
            var l0 = leftHinge.rotation; var r0 = rightHinge.rotation;
            for (float t = 0f; t < openSeconds; t += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0f, 1f, t / openSeconds);
                leftHinge.rotation = Quaternion.Slerp(l0, lTarget * l0, k);
                rightHinge.rotation = Quaternion.Slerp(r0, rTarget * r0, k);
                yield return null;
            }
            leftHinge.rotation = lTarget * l0;
            rightHinge.rotation = rTarget * r0;
            if (doorwayBlocker != null) doorwayBlocker.enabled = false;
        }
    }
}
