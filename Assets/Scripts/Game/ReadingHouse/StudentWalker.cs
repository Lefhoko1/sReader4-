using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Walks the student with the on-screen stick / WASD. Movement is relative to
    /// the camera's facing (flattened), uses a CharacterController so walls, the
    /// dashboard, furniture and the closed doors block him, and turns him to face
    /// where he is going. He always WALKS: one steady pace from the first step to
    /// the last (no speeding up, no running, however far the stick is pushed), and a
    /// change of direction swings his heading round at a gentle turn rate, so he
    /// turns smoothly instead of snapping. Disabled while a reading view is active
    /// (the presenter moves him to his gameplay spot instead).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class StudentWalker : MonoBehaviour
    {
        public TouchJoystick input;
        public Transform viewCamera;
        [Tooltip("His one walking pace (m/s).")]
        public float speed = 1.35f;
        [Tooltip("How fast his heading swings round to a new direction (degrees per second).")]
        public float turnRate = 300f;
        public float turnSharpness = 10f;
        [Range(0f, 0.5f)] public float deadZone = 0.12f;
        public float gravity = -12f;

        Vector3 planarVelocity;
        Vector3 heading;

        CharacterController controller;
        float verticalSpeed;
        Vector3 lastSafe;
        const float fallLimit = 4f;

        void Start() => lastSafe = transform.position;

        void Respawn()
        {
            controller.enabled = false;
            transform.position = lastSafe + Vector3.up * 0.1f;
            controller.enabled = true;
            verticalSpeed = 0f;
        }

        public bool IsMoving { get; private set; }

        /// <summary>Where he is heading (flat, unit; zero before his first step).</summary>
        public Vector3 Heading => heading;

        void Awake() => controller = GetComponent<CharacterController>();

        void OnEnable()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            controller.enabled = true;
        }

        void OnDisable()
        {
            if (controller != null) controller.enabled = false;   // lets other code move the transform freely
            IsMoving = false;
            planarVelocity = Vector3.zero;
        }

        void Update()
        {
            Vector2 stick = input != null ? input.Value : Vector2.zero;
            Vector3 fwd = viewCamera != null ? Vector3.ProjectOnPlane(viewCamera.forward, Vector3.up).normalized : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, fwd);
            Vector3 move = Vector3.zero;
            if (stick.magnitude >= deadZone)
            {
                Vector3 wanted = (fwd * stick.y + right * stick.x).normalized;
                // Swing the heading round (a reversal goes through the side views, never a snap).
                if (heading.sqrMagnitude < 0.0001f) heading = wanted;
                else
                {
                    if (Vector3.Dot(heading, wanted) < -0.999f) heading = Quaternion.Euler(0f, 1f, 0f) * heading;   // exactly opposite: turn about the vertical
                    heading = Vector3.RotateTowards(heading, wanted, turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f);
                    heading = Vector3.ProjectOnPlane(heading, Vector3.up).normalized;
                }
                move = heading;
            }

            // One steady walking pace: no acceleration, no running.
            planarVelocity = move * speed;
            IsMoving = planarVelocity.sqrMagnitude > 0.01f;

            if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -1f;
            verticalSpeed += gravity * Time.deltaTime;
            controller.Move((planarVelocity + Vector3.up * verticalSpeed) * Time.deltaTime);

            // Safety net: remember where he last stood on something; if he ever drops out of the world, put him back.
            if (controller.isGrounded) lastSafe = transform.position;
            else if (transform.position.y < lastSafe.y - fallLimit) Respawn();

            if (move.sqrMagnitude > 0.01f)
            {
                // Smooth, speed-independent turn (critically damped feel); position and scale are never touched here.
                var target = Quaternion.LookRotation(move, Vector3.up) * FacingCorrection;
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 1f - Mathf.Exp(-turnSharpness * Time.deltaTime));
            }
        }

        /// <summary>The model's own "forward" offset, captured once from a known facing (see SetFacingReference).</summary>
        public Quaternion FacingCorrection = Quaternion.identity;

        /// <summary>
        /// Calibrate which way the model faces: given that the model currently faces
        /// <paramref name="worldDirection"/>, store the rotation offset so movement turns him correctly.
        /// </summary>
        public void SetFacingReference(Vector3 worldDirection)
        {
            var look = Quaternion.LookRotation(Vector3.ProjectOnPlane(worldDirection, Vector3.up).normalized, Vector3.up);
            FacingCorrection = Quaternion.Inverse(look) * transform.rotation;
        }
    }
}
