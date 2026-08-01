// ===========================================================================
//  WalkCamera — the camera walks with the reader
// ===========================================================================
//  THE POINT OF THIS FILE. Everything in the river was previously bent to suit
//  one fixed camera: stones re-sized by depth, spacing solved in pixels, the
//  path slid around so nothing hid behind the book. All of that is a way of
//  saying "the far end of the world is too far from the lens". Walking the
//  camera along with the reader answers it directly — the reader is always in
//  the middle of the shot, and the word they are at is always a few metres
//  away, so it is always legible at its natural size.
//
//  Three shots, blended by damping alone (no state machine, no cuts):
//     TRAVEL — behind and above, leading down the road, so you see what's next
//     READ   — closer, framing reader AND the stone they are standing at
//     SEATED — out in front on the water, the reader on the mat, library behind
//
//  The three shots are composed for a 16:9 frame. The game runs portrait, which
//  shows nearly four times less world sideways at the same distance and lens —
//  so the reader fills the frame and the stones fall outside it. SolveShot()
//  fits the authored shot to whatever frame it actually has; see it for the how.
//
//  CINEMACHINE. This is deliberately the same shape Cinemachine works in: a
//  target to follow, an aim point, and damping. If you install Cinemachine,
//  you do not throw this away — you untick `driveCamera` and point a
//  CinemachineCamera at `aimTarget` (which this keeps updated either way):
//
//     • Follow  = the Reader object,  LookAt = WalkAim
//     • Position Composer (or Third Person Follow), damping ~0.4
//     • add a second CinemachineCamera for the READ shot with a higher
//       priority, and enable it from PathWalker.onArrive / disable on onLeave —
//       Cinemachine then blends the two for you.
//   The reason to keep this script until then: it works today, with no package,
//   and proves the idea before you build a rig around it.
// ===========================================================================
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WalkCamera : MonoBehaviour
{
    [Header("Wiring")]
    public PathWalker walker;
    [Tooltip("Kept on the point the shot is composed around — the reader, or the " +
             "midpoint between the reader and the word being read. Point a " +
             "Cinemachine camera's LookAt at this and untick Drive Camera.")]
    public Transform aimTarget;
    [Tooltip("Untick to hand the camera to Cinemachine (or to anything else) " +
             "while still driving Aim Target.")]
    public bool driveCamera = true;

    [Header("Travel shot")]
    public float travelBack = 4.2f;
    public float travelUp = 2.5f;
    public float travelSide = 2.2f;
    public float travelFov = 42f;
    [Tooltip("How far down the road the camera looks — the reason you can see " +
             "the words that are coming.")]
    public float lookAhead = 3.5f;

    [Header("Read shot (standing at a word)")]
    public float readBack = 2.6f;
    public float readUp = 1.7f;
    public float readSide = 2.4f;
    public float readFov = 34f;

    [Header("Seated shot (on the mat)")]
    public float seatFront = 5.0f;
    public float seatUp = 2.6f;
    public float seatSide = 1.6f;
    public float seatFov = 45f;

    [Header("Framing")]
    [Tooltip("One knob for the whole rig: multiplies every shot's back / up / side " +
             "offset. Raise it if the reader crowds the frame, lower it to get " +
             "intimate. 1 = the shot values above, as authored.")]
    public float shotDistance = 1f;
    [Tooltip("The three shots are composed for a 16:9 frame. A portrait phone " +
             "frame is far narrower, so it sees much less of the world sideways — " +
             "which reads as 'the camera is too close' and pushes the stones off " +
             "screen. Tick this and the rig wins that width back automatically.")]
    public bool fitToAspect = true;
    [Tooltip("The aspect the shot values above are composed for.")]
    public float referenceAspect = 16f / 9f;
    [Range(0f, 1f)]
    [Tooltip("How much of the missing width to win back. 1 = the portrait frame " +
             "shows exactly as much world as 16:9 did (very far away on a phone); " +
             "0 = no compensation at all.")]
    public float aspectCompensation = 0.7f;
    [Tooltip("Cap on how far the compensation may dolly out, as a multiple of the " +
             "authored distance.")]
    public float maxPullback = 2.4f;
    [Tooltip("Cap on how wide the compensation may open the lens. Past ~65° the " +
             "perspective distorts; anything the lens can't take, the dolly does.")]
    public float maxFov = 64f;
    [Tooltip("A hard floor: whatever the shot maths asks for, the camera never " +
             "ends up nearer the reader than this, so it can never crowd them.")]
    public float minReaderDistance = 4f;

    [Header("Feel")]
    [Tooltip("Seconds of position damping. Higher = heavier, more filmic.")]
    public float positionDamping = 0.45f;
    public float rotationDamping = 0.28f;
    public float fovDamping = 0.5f;
    [Tooltip("The camera never drops below this height, so it can't dip under " +
             "the sea on the way in.")]
    public float minHeight = 0.9f;
    [Tooltip("Eye height of the reader, in metres.")]
    public float headHeight = 1.55f;

    Camera _cam;
    Vector3 _vel;
    float _fov, _fovVel;

    void Awake() { _cam = GetComponent<Camera>(); _fov = _cam.fieldOfView; }

    void Start()
    {
        if (walker == null) walker = FindAnyObjectByType<PathWalker>();
        EnsureAim();
        Frame(1f);          // snap: no swoop in from wherever the camera was left
    }

    void LateUpdate() { Frame(Time.deltaTime); }

    /// <summary>
    /// Park the camera on the shot it will open with, now, with no damping.
    /// The editor build calls this so the Game view previews the real opening
    /// frame without entering play mode — otherwise the camera keeps whatever
    /// pose it was last left in, which for this scene is the old island vista:
    /// parked level with the river's first stone, so the near stones sit off
    /// the side of the frame and it looks as though they were laid wrong.
    /// Safe in edit mode: the walker has not begun, so this reads the road's
    /// start exactly as the first frame of Play will.
    /// </summary>
    public void Compose()
    {
        if (walker == null) walker = FindAnyObjectByType<PathWalker>();
        if (walker == null) return;
        if (walker.road != null && walker.road.StopCount == 0) walker.road.Rebuild();
        Frame(1f);
    }

    void EnsureAim()
    {
        if (aimTarget != null) return;
        var go = new GameObject("WalkAim");
        aimTarget = go.transform;
    }

    void Frame(float dt)
    {
        if (walker == null) return;
        EnsureAim();
        bool snap = dt >= 1f;

        Vector3 head = walker.transform.position + Vector3.up * headHeight;
        Vector3 fwd = Flat(walker.Heading);
        if (fwd.sqrMagnitude < 0.0001f) fwd = Flat(walker.transform.forward);
        Vector3 lateral = Vector3.Cross(Vector3.up, fwd).normalized;

        // The road picks a bank; the camera takes the other one, so the words sit
        // between the lens and the reader and never end up behind their head.
        float bank = walker.road != null ? -walker.road.SideSign : 1f;

        // Each shot is an origin, an offset from it, and a lens. Keeping the offset
        // separate from the origin is what lets the framing solve below scale the
        // whole rig without any shot having to know about it.
        Vector3 origin, offset, aim; float wantFov;

        if (walker.Seated)
        {
            Vector3 f = Flat(walker.transform.forward);
            aim = walker.transform.position + Vector3.up * (headHeight * 0.65f);
            origin = aim;
            offset = f * seatFront + Vector3.up * seatUp +
                     Vector3.Cross(Vector3.up, f).normalized * seatSide;
            wantFov = seatFov;
        }
        else if (walker.CurrentStone != null)
        {
            Vector3 word = walker.CurrentStone.position + Vector3.up *
                           (0.55f * Mathf.Max(0.2f, walker.CurrentStone.lossyScale.y));
            aim = Vector3.Lerp(head, word, 0.5f);
            origin = aim;
            offset = -fwd * readBack + Vector3.up * readUp + lateral * (bank * readSide);
            wantFov = readFov;
        }
        else
        {
            aim = head + fwd * lookAhead - Vector3.up * 0.25f;
            origin = head;
            offset = -fwd * travelBack + Vector3.up * travelUp +
                     lateral * (bank * travelSide);
            wantFov = travelFov;
        }

        float pull;
        wantFov = SolveShot(wantFov, out pull);
        Vector3 wantPos = origin + offset * pull;

        // The floor. The shot maths can put the lens anywhere; this is the promise
        // that it is never in the reader's face — back off along the line the shot
        // already chose, so the angle it composed survives.
        Vector3 fromReader = wantPos - head;
        float near = Mathf.Max(0.1f, minReaderDistance);
        if (fromReader.sqrMagnitude < near * near && fromReader.sqrMagnitude > 1e-4f)
            wantPos = head + fromReader.normalized * near;

        wantPos.y = Mathf.Max(wantPos.y, minHeight);
        aimTarget.position = aim;
        if (!driveCamera) return;

        transform.position = snap
            ? wantPos
            : Vector3.SmoothDamp(transform.position, wantPos, ref _vel,
                                 Mathf.Max(0.01f, positionDamping));

        var look = Quaternion.LookRotation((aim - transform.position).normalized, Vector3.up);
        transform.rotation = snap
            ? look
            : Quaternion.Slerp(transform.rotation, look,
                               1f - Mathf.Exp(-dt / Mathf.Max(0.01f, rotationDamping)));

        _fov = snap ? wantFov
                    : Mathf.SmoothDamp(_fov, wantFov, ref _fovVel,
                                       Mathf.Max(0.01f, fovDamping));
        if (_cam == null) _cam = GetComponent<Camera>();
        _cam.fieldOfView = _fov;
    }

    /// <summary>
    /// Fit a shot authored for <see cref="referenceAspect"/> to the frame we
    /// actually have. A frame that is N times narrower shows N times less world
    /// sideways at the same distance and lens — on a portrait phone N is nearly
    /// four, which is why a shot that reads fine in the Scene view arrives with
    /// the reader filling it and the stones outside it.
    ///
    /// The width is won back with a MIX of dollying out and opening the lens,
    /// because all of it on either one alone looks wrong: a lens that wide bends
    /// the world, a dolly that long loses the reader. Split evenly (the sqrt),
    /// then whatever <see cref="maxFov"/> refuses to take, the dolly takes.
    /// </summary>
    /// <returns>The vertical FOV to use; <paramref name="pull"/> is the multiplier
    /// for the shot's offset.</returns>
    float SolveShot(float fov, out float pull)
    {
        pull = Mathf.Max(0.1f, shotDistance);
        if (_cam == null) _cam = GetComponent<Camera>();

        float aspect = _cam != null ? _cam.aspect : referenceAspect;
        if (!fitToAspect || aspect <= 0.01f || aspect >= referenceAspect) return fov;

        float missing = Mathf.Pow(referenceAspect / aspect, Mathf.Clamp01(aspectCompensation));
        float dolly = Mathf.Clamp(Mathf.Sqrt(missing), 1f, Mathf.Max(1f, maxPullback));

        float wantTan = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * (missing / dolly);
        float wide = Mathf.Min(2f * Mathf.Atan(wantTan) * Mathf.Rad2Deg,
                               Mathf.Max(fov, maxFov));

        float gotTan = Mathf.Tan(wide * 0.5f * Mathf.Deg2Rad);
        if (gotTan > 0.001f) dolly *= Mathf.Max(1f, wantTan / gotTan);

        pull *= dolly;
        return wide;
    }

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v.sqrMagnitude > 1e-6f ? v.normalized : v; }
}
