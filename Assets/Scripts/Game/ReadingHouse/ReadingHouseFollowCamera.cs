using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The explore camera: one camera behaviour for everything outside the
    /// reading views, so the picture never jumps or zooms.
    ///   • Follow — a fixed offset behind the student (always looking into the
    ///     house), critically damped so it eases instead of snapping.
    ///   • Pose — glide to a framed shot (the opening wide shot, the sign-in shot).
    /// Dragging empty screen space swings the view round (<see cref="Orbit"/>): round the student
    /// while following, round the shot's aim point on a pose. It stays where it is let go: nothing
    /// swings it back to the front.
    /// One field of view throughout, and no wall collision: inside the house the
    /// front wall fades out instead (FrontWallCutaway), so the camera keeps the
    /// same distance and the student keeps the same size on screen. The house
    /// itself is never moved or scaled — framing is done with the camera only.
    /// </summary>
    public sealed class ReadingHouseFollowCamera : MonoBehaviour
    {
        public Camera viewCamera;
        public Transform target;
        [Tooltip("Direction the camera looks while following (flattened): into the house.")]
        public Vector3 viewDirection = Vector3.forward;
        public float distance = 5.4f;
        public float height = 3.0f;
        public float lookHeight = 1.5f;
        public float fieldOfView = 40f;

        [Header("Smoothing")]
        public float followSmoothTime = 0.16f;
        public float poseSmoothTime = 0.55f;
        public float rotationSharpness = 9f;

        [Header("Dragging the view round")]
        [Tooltip("How far round the student the follow view may swing (degrees each way); the house's walls stay out of the way.")]
        public float followYawLimit = 80f;
        public Vector2 heightRange = new Vector2(1.4f, 6f);

        float followYaw, followHeight = float.NaN, poseYaw;

        enum Mode { Off, Follow, Pose }
        Mode mode = Mode.Off;
        Vector3 posePosition, poseAim;
        Vector3 velocity, aimVelocity, smoothedAim;
        float fovVelocity, blendSmooth;
        Vector2 shiftVelocity;

        public bool Engaged => mode != Mode.Off;

        /// <summary>True once the camera has arrived (used to grab the frosted backdrop at rest).</summary>
        public bool Settled => mode != Mode.Off && velocity.sqrMagnitude < 0.0004f && aimVelocity.sqrMagnitude < 0.0004f;

        public void Follow() { Begin(Mode.Follow); }

        /// <summary>Swing the view round by a drag: degrees about the vertical, and metres up / down.</summary>
        public void Orbit(float yawDegrees, float heightMetres)
        {
            if (mode == Mode.Follow)
            {
                followYaw = Mathf.Clamp(followYaw + yawDegrees, -followYawLimit, followYawLimit);
                if (float.IsNaN(followHeight)) followHeight = height;
                followHeight = Mathf.Clamp(followHeight + heightMetres, heightRange.x, heightRange.y);
            }
            else if (mode == Mode.Pose) poseYaw += yawDegrees;
        }

        public void HoldPose(Vector3 position, Vector3 aim)
        {
            poseYaw = 0f;                                          // a new shot starts as framed
            posePosition = position;
            poseAim = aim;
            Begin(Mode.Pose);
        }

        public void SnapToPose(Vector3 position, Vector3 aim)
        {
            HoldPose(position, aim);
            Prepare();
            var dir = (aim - position).normalized;
            viewCamera.transform.SetPositionAndRotation(position, Quaternion.LookRotation(Vector3.ProjectOnPlane(dir, Vector3.up).normalized, Vector3.up));
            float pitch = Mathf.Asin(Mathf.Clamp(dir.y, -0.99f, 0.99f));
            float half = fieldOfView * 0.5f * Mathf.Deg2Rad;
            float tTop = Mathf.Tan(pitch + half), tBottom = Mathf.Tan(pitch - half), span = tTop - tBottom;
            viewCamera.fieldOfView = 2f * Mathf.Atan(span * 0.5f) * Mathf.Rad2Deg;
            viewCamera.lensShift = new Vector2(0f, (tTop + tBottom) * 0.5f / span);
            smoothedAim = aim;
            velocity = aimVelocity = Vector3.zero;
        }

        public void Release() => mode = Mode.Off;

        /// <summary>Kept for older callers: engage = follow, disengage = release.</summary>
        public void Engage(bool on) { if (on) Follow(); else Release(); }

        void Begin(Mode m)
        {
            if (mode == Mode.Off)
            {
                // Taking over from another camera driver: start from where the camera is now.
                Prepare();
                smoothedAim = viewCamera.transform.position + viewCamera.transform.forward * 6f;
                velocity = aimVelocity = Vector3.zero;
            }
            // A change of mode always glides with the slower pose timing, then tightens up.
            blendSmooth = poseSmoothTime;
            mode = m;
        }

        void Prepare()
        {
            viewCamera.usePhysicalProperties = true;               // same projection mode as the reading rig: no pop when handing over
            viewCamera.gateFit = Camera.GateFitMode.Vertical;
        }

        void LateUpdate()
        {
            if (mode == Mode.Off || viewCamera == null) return;
            Vector3 desiredPos, desiredAim;
            if (mode == Mode.Follow && target != null)
            {
                var back = Quaternion.AngleAxis(followYaw, Vector3.up) * Vector3.ProjectOnPlane(viewDirection, Vector3.up).normalized;
                desiredAim = target.position + Vector3.up * lookHeight;
                desiredPos = target.position - back * distance + Vector3.up * (float.IsNaN(followHeight) ? height : followHeight);
            }
            else
            {
                desiredAim = poseAim;
                desiredPos = poseAim + Quaternion.AngleAxis(poseYaw, Vector3.up) * (posePosition - poseAim);
            }

            float targetSmooth = mode == Mode.Follow ? followSmoothTime : poseSmoothTime;
            blendSmooth = Mathf.MoveTowards(blendSmooth, targetSmooth, Time.deltaTime * 0.6f);

            var t = viewCamera.transform;
            t.position = Vector3.SmoothDamp(t.position, desiredPos, ref velocity, blendSmooth);
            smoothedAim = Vector3.SmoothDamp(smoothedAim, desiredAim, ref aimVelocity, blendSmooth);

            // Keep verticals straight: the camera itself stays level (yaw only) and the lens is shifted
            // to frame exactly what a tilted camera would see — walls stand upright instead of leaning.
            Vector3 dir = (smoothedAim - t.position).normalized;
            Vector3 flat = Vector3.ProjectOnPlane(dir, Vector3.up);
            var level = Quaternion.LookRotation(flat.sqrMagnitude > 1e-6f ? flat.normalized : t.forward, Vector3.up);
            t.rotation = Quaternion.Slerp(t.rotation, level, 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));

            float pitch = Mathf.Asin(Mathf.Clamp(dir.y, -0.99f, 0.99f));
            float half = fieldOfView * 0.5f * Mathf.Deg2Rad;
            float tTop = Mathf.Tan(pitch + half), tBottom = Mathf.Tan(pitch - half);
            float span = tTop - tBottom;
            float verticalFov = 2f * Mathf.Atan(span * 0.5f) * Mathf.Rad2Deg;
            var shift = new Vector2(0f, (tTop + tBottom) * 0.5f / span);
            viewCamera.fieldOfView = Mathf.SmoothDamp(viewCamera.fieldOfView, verticalFov, ref fovVelocity, 0.12f);
            viewCamera.lensShift = Vector2.SmoothDamp(viewCamera.lensShift, shift, ref shiftVelocity, 0.12f);
        }
    }
}
