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
//  AND ONE RULE THAT OUTRANKS ALL THREE: inside the library, the lens stays
//  inside the library. Every shot here is authored as a distance in open air —
//  the reveal asks to be five metres back from the middle of a room four metres
//  deep — and a shot that cannot have what it asked for gets the next best thing
//  outdoors: a position through the wall, filming the outside of the building the
//  reader is standing in. You cannot be in a house and see the outside of it, so
//  the room is measured (its floor tiles are its plan) and the camera is held in
//  it. See InsideTheHall, which runs last, after every other rule has had its say.
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

    [Header("Backdrop — what stays behind the reader")]
    [Tooltip("The library. The walk shots are composed along reader -> this, so it " +
             "sits behind them in every frame instead of swinging in and out as the " +
             "river bends. Empty = the old behaviour, following the road's tangent.")]
    public Transform backdrop;
    [Range(0f, 1f)]
    [Tooltip("1 = the backdrop is always dead behind the reader. 0 = pure road " +
             "heading (the camera yaws with every bend). In between leans one way " +
             "or the other; 0.85 keeps the library in shot while still reading as a " +
             "camera that follows the walk.")]
    public float backdropWeight = 0.85f;
    [Tooltip("How close the reader may get to the backdrop before the shot stops " +
             "using it. Right underneath it the direction is meaningless and would " +
             "spin the camera.")]
    public float backdropMinRange = 4f;
    [Tooltip("Work the walk shots' direction out ONCE and then hold it. The camera " +
             "still follows the reader — it keeps a constant offset from them, so it " +
             "translates as they walk — but it never turns, whichever way THEY turn. " +
             "This is the difference between a camera that follows someone and a " +
             "camera that is strapped to their shoulders.")]
    public bool lockWalkDirection = true;
    [Tooltip("Spin the held heading, in degrees. The lock takes the river's own " +
             "direction, which is the honest default — this is for taste, when you " +
             "want the walk running up the screen at a slight angle rather than " +
             "straight away from the lens.")]
    public float walkYawOffset;

    [Range(0f, 1f)]
    [Tooltip("How far the READ shot's aim slides from the reader toward the word " +
             "they are standing at. Low keeps the horizon still; 0.5 was the old " +
             "value and it re-framed on every stone.")]
    public float wordBias = 0.22f;

    [Header("Diorama shot — the whole island in frame")]
    [Tooltip("Frame the WHOLE scene — library, every word stone, the reader — as one " +
             "picture, the way it reads in Blender, instead of following the reader " +
             "around. The camera then barely moves: it only re-fits as the sentence " +
             "changes the length of the river. Overridden by the book shot indoors.")]
    public bool dioramaShot;
    [Range(0.3f, 1f)]
    [Tooltip("How much of the frame's tight axis the whole island fills. 0.85 leaves " +
             "a little air around it; 1.0 crops to the edges.")]
    public float dioramaFill = 0.85f;
    [Tooltip("Degrees above the island.")]
    public float dioramaPitch = 30f;
    [Tooltip("Degrees around the island from the default view, which looks up the " +
             "river at the library.")]
    public float dioramaYaw;
    public float dioramaFov = 42f;

    [Tooltip("Work out the viewing angle once and then NEVER turn the camera again. " +
             "The shot may slide and zoom to follow the reader, but the world holds " +
             "still and only the reader turns — which is what stops the island " +
             "rolling about while you are trying to read words off it.")]
    public bool dioramaLockRotation = true;
    [Range(0f, 1f)]
    [Tooltip("How far the frame slides from the middle of the island toward the " +
             "reader. 0 = the whole scene stays centred; 1 = the reader is centred " +
             "and the island drifts off. The camera only TRANSLATES to do this.")]
    public float dioramaFollow = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("How much closer the shot pushes in on the reader. 0 = always the full " +
             "island; 1 = framed on the reader alone. This is the 'scale a little' " +
             "knob — it is a dolly along the fixed axis, never a turn.")]
    public float dioramaZoom = 0.35f;
    [Tooltip("The size of the reader's own shot, in metres, when Diorama Zoom is 1.")]
    public float dioramaCharacterFrame = 5f;

    [Header("Book shot (standing at the lectern)")]
    [Tooltip("While this is set it OUTRANKS the three walk shots: the camera comes " +
             "round over the reader's shoulder and frames whatever it points at — " +
             "the open book. The reading loop sets it as the reader walks up to the " +
             "lectern and clears it when they leave, so the move is part of the walk " +
             "rather than a cut.")]
    public Transform focus;
    [Tooltip("How far the lens sits from the book, in metres. FIXED — it does not " +
             "move with the aspect. The book stands at the edge of the island, so a " +
             "shot that dollies back to suit a narrow frame reverses into the hill; " +
             "the lens stays here and the FOV does the work instead.")]
    public float focusDistance = 2.4f;
    [Tooltip("How much further than Focus Distance the lens may back off when the " +
             "lens alone cannot make the book small enough — as a multiple. Only a " +
             "narrow frame ever needs it, and the shot slides forward again if the " +
             "hill is in the way.")]
    public float focusMaxPullback = 2.2f;
    [Range(0.15f, 0.95f)]
    [Tooltip("How much of the frame's TIGHT axis the book fills — the whole point of " +
             "this shot. A 720x1520 phone shows about four times less world sideways " +
             "than the editor window this was composed in, so the FOV is SOLVED from " +
             "the book's measured size and the frame we actually have. 0.6 = the book " +
             "is a bit over half the narrow dimension, on any device.")]
    public float focusFill = 0.4f;
    [Tooltip("Degrees above the book the camera sits. Angles, not metres — an angle " +
             "means the same thing at every distance and on every screen.")]
    public float focusPitch = 24f;
    [Tooltip("Degrees round from directly behind the reader, so you see the page " +
             "past their shoulder rather than through their head.")]
    public float focusYaw = 26f;
    [Tooltip("Bounds on the solved lens. The floor stops a wide editor window from " +
             "reading as a telephoto; the ceiling stops a very narrow phone from " +
             "bending the page with a fisheye — past the ceiling the book simply " +
             "fills a little more of the frame than asked, which is harmless.")]
    public float focusMinFov = 26f;
    public float focusMaxFov = 62f;
    [Tooltip("Radius of the thing being framed, in metres. 0 = measure it from the " +
             "target's renderers (what you normally want).")]
    public float focusRadius = 0f;

    [Header("Travel shot")]
    public float travelBack = 4.2f;
    public float travelUp = 2.5f;
    public float travelSide = 2.2f;
    public float travelFov = 42f;
    [Tooltip("How far down the road the camera looks — the reason you can see " +
             "the words that are coming.")]
    public float lookAhead = 3.5f;

    [Header("Read shot (standing at a word)")]
    [Tooltip("Don't change the shot at all when the reader stops at a word. The " +
             "read shot sits at a different distance from the travel shot, so " +
             "arriving at a stone dollies the lens — and because it happens the " +
             "instant they stop, it reads as the world lurching just as you are " +
             "trying to read. Holding the shot means stopping looks like stopping. " +
             "Untick to get the separate closer framing below.")]
    public bool holdShotAtStones = true;
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

    [Header("Staying out of trouble")]
    [Tooltip("Near clip plane, in metres. 0 leaves the Camera's own. The default " +
             "of 0.3 is sized for a world you stand back from; indoors the reader " +
             "walks right up to a desk 0.56 m high and the near plane eats the front " +
             "of it, which reads as the furniture being enormous and half missing.")]
    public float nearClip = 0.05f;
    [Tooltip("What counts as solid. The camera stops short of anything on these " +
             "layers rather than sliding inside it.")]
    public LayerMask collideWith = ~0;
    [Tooltip("How fat the camera is when testing for walls, in metres.")]
    public float collisionRadius = 0.35f;
    [Tooltip("Extra clearance kept off whatever it stopped against.")]
    public float collisionBuffer = 0.15f;
    [Tooltip("The reader is never allowed further than this from the middle of the " +
             "frame, in degrees. The shot may lead down the road, but not so far " +
             "that it walks off without them. 0 disables the guard.")]
    public float maxOffCentre = 18f;
    [Tooltip("Metres the camera keeps above the reader's feet, on top of Min " +
             "Height — the reason it cannot sink to sea level and film the ocean.")]
    public float minAboveReader = 1.2f;
    [Tooltip("Eye height above the reader's feet, IF Size From The Reader is off. " +
             "Indoors the camera sits here, looking the way they face — you see the " +
             "library through their eyes.")]
    public float eyeHeight = 1.6f;
    [Tooltip("A little in front of the face, so the reader's own head is not in " +
             "the way of their own view. It is not enough on its own — the body is " +
             "hidden as well; see Hide The Reader In First Person.")]
    public float eyeForward = 0.28f;
    [Tooltip("MEASURE THE READER RATHER THAN REMEMBERING HOW BIG THEY WERE. Eye " +
             "Height and Head Height below are the same person written down twice " +
             "more, so making the reader smaller used to be a three-place edit and " +
             "missing one left the camera hovering where their head had been. With " +
             "this on, both come off the reader's actual height and the one dial on " +
             "PlaceholderActor moves the whole rig.")]
    public bool sizeFromTheReader = true;
    [Tooltip("Hide the reader's own body while the camera is at their eyes. You " +
             "cannot see your own head: the lens sits inside the model, so without " +
             "this the frame fills with the inside of it — which looks exactly like " +
             "a character several times too big.")]
    public bool hideReaderInFirstPerson = true;
    [Tooltip("Wider lens indoors, to see any of the room from that short a boom. " +
             "This is the VERTICAL field, which is the wide axis on a portrait " +
             "phone — see Indoor Horizontal Fov for the axis that actually decides " +
             "how much of the room you see.")]
    public float indoorFov = 62f;
    [Tooltip("How much of the room the lens shows ACROSS the frame, in degrees. " +
             "On a portrait phone the horizontal field is the narrow one: 62° " +
             "vertical on a 720x1520 screen is only 32° across, which is a keyhole " +
             "— you stand in a library and see one shelf. This is solved into the " +
             "vertical field from the frame we actually have, so the room reads the " +
             "same on a phone as it does in the editor.")]
    public float indoorHorizontalFov = 52f;
    [Tooltip("Ceiling on the solved indoor lens. Past this the room bends.")]
    public float indoorMaxFov = 78f;
    [Tooltip("Name prefix of the library floor tiles. Their combined bounds ARE " +
             "the room, and standing in them is what counts as being inside.")]
    public string hallFloorPrefix = "Floor_";

    [Tooltip("THE WALLS ARE SOLID FOR THE CAMERA TOO. Inside the library the lens " +
             "is kept within the room — you cannot be in a building and see the " +
             "outside of it. Untick only to debug where a shot wanted to go.")]
    public bool stayInsideTheHall = true;
    [Tooltip("Height of the room above its floor, in metres — the underside of the " +
             "ceiling. The floor tiles give the camera the room's PLAN; nothing in " +
             "the scene gives it the height, because the imported building carries " +
             "no colliders, so it is a number. Tools ▸ Great Library ▸ Library ▸ " +
             "7 measures it off the model and writes it here.")]
    public float hallHeight = 3.4f;
    [Tooltip("How far off the walls the lens is held, in metres. Zero would let it " +
             "sit in the plaster, where the near plane clips through to the island.")]
    public float wallClearance = 0.45f;
    [Tooltip("How far under the ceiling the lens is held, in metres.")]
    public float ceilingClearance = 0.3f;
    [Range(0.1f, 0.6f)]
    [Tooltip("Metres over which the eye view takes over at the doorway. SHORT on " +
             "purpose: half way between the outdoor boom and the eye is a position " +
             "that is neither, six metres back and four up scaled down — which is " +
             "inside the wall. A long, pretty blend spends its whole length in " +
             "masonry. Cross quickly; the damping still keeps it from being a cut. " +
             "The ramp runs from the NEAREST WALL, so its length is also the width " +
             "of the band around the room where the reader counts as half outside — " +
             "which is why it is held to 0.6 m however it is set: a metre of it in a " +
             "hall four metres deep leaves nowhere that reads as inside at all.")]
    public float thresholdBlend = 0.35f;

    [Header("Feel")]
    [Tooltip("Seconds of position damping. Higher = heavier, more filmic.")]
    public float positionDamping = 0.45f;
    public float rotationDamping = 0.28f;
    public float fovDamping = 0.5f;
    [Tooltip("The camera never drops below this height, so it can't dip under " +
             "the sea on the way in.")]
    public float minHeight = 0.9f;
    [Tooltip("Eye height of the reader, in metres, IF Size From The Reader is off.")]
    public float headHeight = 1.55f;

    // ── how big the reader actually is ──────────────────────────────────────

    float _readerHeight = -1f;

    /// <summary>
    /// The reader's height, measured off the reader. Their own component knows it;
    /// failing that their renderers do. Cached, because it is a property of the
    /// model and not of the frame — and because a walking placeholder's bounds bob
    /// with the stride, which would wobble every shot built on it.
    /// </summary>
    public float ReaderHeight
    {
        get
        {
            if (_readerHeight > 0f) return _readerHeight;
            _readerHeight = 0f;
            if (walker == null) return 0f;

            var actor = walker.GetComponentInChildren<PlaceholderActor>(true);
            if (actor != null) { _readerHeight = Mathf.Max(0.2f, actor.height); return _readerHeight; }

            var rends = Renderers(walker.transform);
            if (rends.Length == 0) return 0f;
            var b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            _readerHeight = Mathf.Max(0f, b.max.y - walker.transform.position.y);
            return _readerHeight;
        }
    }

    /// <summary>Forget the reader's measured size — the editor rebuilds them.</summary>
    public void ResetReaderSize() { _readerHeight = -1f; _actorLooked = false; }

    /// <summary>Where the shots that frame the reader aim: the top of them.</summary>
    public float HeadHeight =>
        sizeFromTheReader && ReaderHeight > 0.2f ? ReaderHeight * 0.89f : headHeight;

    /// <summary>Where the first-person lens sits.</summary>
    public float EyeHeight =>
        sizeFromTheReader && ReaderHeight > 0.2f ? ReaderHeight * 0.93f : eyeHeight;

    Camera _cam;
    Vector3 _vel;
    float _fov, _fovVel;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        _fov = _cam.fieldOfView;
        if (nearClip > 0.0001f) _cam.nearClipPlane = nearClip;
    }

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
        ResetHall();                    // the library may have just been moved
        ResetReaderSize();              // and the reader may have just been resized
        if (_cam == null) _cam = GetComponent<Camera>();
        if (_cam != null && nearClip > 0.0001f) _cam.nearClipPlane = nearClip;
        Frame(1f);
    }

    Bounds _libBounds;
    bool _libMeasured;

    /// <summary>
    /// Fit the WHOLE diorama — library, every word stone, the reader — into the
    /// frame we actually have, from a fixed angle looking up the river.
    ///
    /// This is a different promise from the follow shots: those keep the reader
    /// legible and let the world fall outside the frame, and on a portrait phone
    /// almost all of it does. Here the picture is the thing, so the distance is
    /// solved from the scene's own bounding radius against the TIGHT frame axis —
    /// horizontal on a phone — and the camera then sits still while the reader
    /// walks across it.
    /// </summary>
    bool Diorama(out Vector3 centre, out Vector3 dir, out float dist)
    {
        centre = Vector3.zero; dir = Vector3.forward; dist = 10f;
        if (walker == null) return false;

        // the library, measured once
        if (!_libMeasured && backdrop != null)
        {
            var rends = Renderers(backdrop);
            if (rends.Length == 0 && backdrop.parent != null) rends = Renderers(backdrop.parent);
            if (rends.Length > 0)
            {
                var lb = rends[0].bounds;
                foreach (var r in rends) lb.Encapsulate(r.bounds);
                _libBounds = lb; _libMeasured = true;
            }
        }

        var b = _libMeasured ? _libBounds
                             : new Bounds(walker.transform.position, Vector3.one * 4f);
        b.Encapsulate(walker.transform.position + Vector3.up * HeadHeight);

        Vector3 firstStone = Vector3.zero; bool haveStone = false;
        if (walker.road != null)
        {
            for (int i = 0; i < walker.road.StopCount; i++)
            {
                var s = walker.road.Stone(i);
                if (s == null) continue;
                b.Encapsulate(s.position);
                if (!haveStone) { firstStone = s.position; haveStone = true; }
            }
        }

        float radius = Mathf.Max(1f, b.extents.magnitude);

        // ---- the viewing angle, decided ONCE --------------------------------
        // Re-deriving it every frame is what made the world turn: the axis is
        // measured off the scene's own contents, and those move. Solve it once and
        // the camera never rotates again — it only slides and dollies.
        if (dioramaLockRotation && _angleSet)
        {
            dir = _lockedDir;
        }
        else
        {
            // Look up the river at the library: the axis from the building out to
            // the far end of the walk. With no stones yet, fall back to the reader.
            Vector3 axis = Flat(haveStone
                ? firstStone - (_libMeasured ? _libBounds.center : b.center)
                : walker.transform.position - b.center);
            if (axis.sqrMagnitude < 0.01f) axis = Vector3.back;
            axis.Normalize();

            Vector3 back = Quaternion.AngleAxis(dioramaYaw, Vector3.up) * axis;
            float pitch = dioramaPitch * Mathf.Deg2Rad;
            dir = (back * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch)).normalized;
            _lockedDir = dir; _angleSet = true;
        }

        // ---- what the frame is centred on -----------------------------------
        // Sliding the centre toward the reader moves the camera; because the
        // direction above is fixed, moving it can only ever be a translation.
        Vector3 reader = walker.transform.position + Vector3.up * (HeadHeight * 0.6f);
        centre = Vector3.Lerp(b.center, reader, Mathf.Clamp01(dioramaFollow));

        if (_cam == null) _cam = GetComponent<Camera>();
        float aspect = _cam != null && _cam.aspect > 0.01f ? _cam.aspect : referenceAspect;
        float vHalf = Mathf.Max(1f, dioramaFov) * 0.5f * Mathf.Deg2Rad;
        float hHalf = Mathf.Atan(Mathf.Tan(vHalf) * aspect);

        // Fit the box on EACH SCREEN AXIS, not a sphere against the tightest one.
        // An island with a river trailing off it is a tall, narrow composition, and
        // a portrait phone is a tall, narrow frame — they suit each other. Fitting
        // a bounding SPHERE to the horizontal angle throws that away: it reserves
        // as much width as the thing is long, and everything ends up two or three
        // times smaller than it needs to be. Hence "why is it all so tiny".
        Quaternion rot = Quaternion.LookRotation(-dir, Vector3.up);
        Vector3 rightV = rot * Vector3.right, upV = rot * Vector3.up, fwdV = -dir;

        float halfW = 0f, halfH = 0f, halfD = 0f;
        for (int i = 0; i < 8; i++)
        {
            Vector3 v = new Vector3((i & 1) == 0 ? b.min.x : b.max.x,
                                    (i & 2) == 0 ? b.min.y : b.max.y,
                                    (i & 4) == 0 ? b.min.z : b.max.z) - centre;
            halfW = Mathf.Max(halfW, Mathf.Abs(Vector3.Dot(v, rightV)));
            halfH = Mathf.Max(halfH, Mathf.Abs(Vector3.Dot(v, upV)));
            halfD = Mathf.Max(halfD, Mathf.Abs(Vector3.Dot(v, fwdV)));
        }

        float fill = Mathf.Clamp(dioramaFill, 0.2f, 1f);
        float wide = Mathf.Max(halfW / Mathf.Tan(hHalf), halfH / Mathf.Tan(vHalf)) / fill
                     + halfD;                 // clear the near half of the island

        // The reader's own shot: the same solve against a small box around them.
        float cf = Mathf.Max(0.5f, dioramaCharacterFrame) * 0.5f;
        float near = Mathf.Max(cf / Mathf.Tan(hHalf), cf / Mathf.Tan(vHalf)) / fill;

        dist = Mathf.Lerp(wide, near, Mathf.Clamp01(dioramaZoom));
        dist = Mathf.Max(dist, radius * 0.25f);
        return true;
    }

    /// <summary>
    /// Forget the locked viewing angle so the next frame works it out again. For
    /// the editor, and for whenever the world itself is rebuilt underneath it.
    /// </summary>
    public void ResetDioramaAngle() { _angleSet = false; }

    Vector3 _lockedDir;
    bool _angleSet;

    /// <summary>
    /// The forward the walk shots are built on: the road's tangent bent toward
    /// "the library is that way". Falls back to the road alone when there is no
    /// backdrop, or when the reader is close enough to it that the direction stops
    /// meaning anything.
    /// </summary>
    Vector3 Backdrop(Vector3 road)
    {
        if (backdrop == null || backdropWeight <= 0.001f) return road;

        Vector3 to = Flat(backdrop.position - walker.transform.position);
        float d = to.magnitude;
        if (d < Mathf.Max(0.5f, backdropMinRange)) return road;

        return Vector3.Slerp(road, to / d, Mathf.Clamp01(backdropWeight)).normalized;
    }

    Vector3 _walkDir;
    bool _walkDirSet;

    /// <summary>
    /// The direction the walk shots are built on, held FIXED once it is known.
    ///
    /// Every shot below is an offset from the reader along this. Recomputing it
    /// each frame is what turns the camera when the reader turns — they face a
    /// stone, the basis swings, and the whole world rotates behind them. Solving it
    /// once leaves a constant world-space offset instead: the camera slides along
    /// with the reader and holds its heading, which is what "follow them but don't
    /// turn with them" actually means.
    /// </summary>
    Vector3 WalkDirection(Vector3 road)
    {
        if (!lockWalkDirection) return Backdrop(road);
        if (_walkDirSet) return _walkDir;

        // Take it from the ROAD, never from the reader. The first frame this runs,
        // the reader is usually parked at the library door facing INWARD — and
        // while they are off-road, Heading reports their own facing. Locking that
        // leaves them walking across the screen for the rest of the game, with the
        // island off to one side. The river's own direction is the honest answer,
        // and it does not care where anybody is standing.
        Vector3 d = RiverHeading();
        if (d.sqrMagnitude > 1e-4f)
        {
            _walkDir = Quaternion.AngleAxis(walkYawOffset, Vector3.up) * d.normalized;
            _walkDirSet = true;
            return _walkDir;
        }

        // No river laid out yet — use the live direction, but do NOT lock it in.
        // Committing to a guess made before the world exists is the whole bug.
        return Backdrop(road);
    }

    /// <summary>
    /// Which way the walk runs, measured off the stones themselves: first to last.
    /// Zero while there is no river.
    /// </summary>
    Vector3 RiverHeading()
    {
        var r = walker != null ? walker.road : null;
        if (r == null || r.StopCount < 2) return Vector3.zero;

        var a = r.Stone(0);
        var b = r.Stone(r.StopCount - 1);
        if (a == null || b == null) return Vector3.zero;
        return Flat(b.position - a.position);
    }

    /// <summary>Forget the held heading — for the editor, or a rebuilt world.</summary>
    public void ResetWalkDirection() { _walkDirSet = false; }

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

        Vector3 head = walker.transform.position + Vector3.up * HeadHeight;
        Vector3 road = Flat(walker.Heading);
        if (road.sqrMagnitude < 0.0001f) road = Flat(walker.transform.forward);
        road.Normalize();

        // The direction the WALK shots are built around. Following the road's own
        // tangent means the camera yaws with every bend of the river, and the
        // island swings off the edge of frame and back — which is the one thing the
        // shot must never do, because the library is what the walk is FOR. Aiming
        // along reader -> library instead keeps it behind them in every frame, and
        // that direction turns slowly, so the horizon stops rolling about.
        Vector3 fwd = WalkDirection(road);
        Vector3 lateral = Vector3.Cross(Vector3.up, fwd).normalized;

        // The road picks a bank; the camera takes the other one, so the words sit
        // between the lens and the reader and never end up behind their head.
        float bank = walker.road != null ? -walker.road.SideSign : 1f;

        // Each shot is an origin, an offset from it, and a lens. Keeping the offset
        // separate from the origin is what lets the framing solve below scale the
        // whole rig without any shot having to know about it.
        Vector3 origin, offset, aim; float wantFov;

        // The authored travel offset, kept to hand: it is also the ANGLE every
        // other walk shot borrows while the heading is locked.
        Vector3 travelOffset = -fwd * travelBack + Vector3.up * travelUp +
                               lateral * (bank * travelSide);

        bool solved = false;

        if (focus != null)
        {
            // Composed around the BOOK, not around the reader. Splitting the
            // difference between the two looks right only once the reader has
            // arrived; while they are still walking in it points the lens at the
            // empty ground between them, and the thing the shot exists to show
            // sits off at the edge of frame.
            Vector3 page = focus.position;
            Vector3 f = Flat(page - walker.transform.position);
            if (f.sqrMagnitude < 0.0001f) f = Flat(-focus.forward);
            if (f.sqrMagnitude < 0.0001f) f = fwd;
            f.Normalize();

            // Behind the reader, swung round a bit, lifted a bit — all in DEGREES,
            // which mean the same thing on every screen.
            Vector3 back = Quaternion.AngleAxis(focusYaw, Vector3.up) * -f;
            float pitch = focusPitch * Mathf.Deg2Rad;
            Vector3 dir = (back * Mathf.Cos(pitch) + Vector3.up * Mathf.Sin(pitch)).normalized;

            aim = page;
            origin = page;
            wantFov = SolveFocus(out float dist);
            offset = dir * dist;
            solved = true;      // lens and distance already account for the frame
        }
        else if (dioramaShot && Diorama(out Vector3 dCentre, out Vector3 dDir, out float dDist))
        {
            aim = dCentre;
            origin = dCentre;
            offset = dDir * dDist;
            wantFov = dioramaFov;
            solved = true;              // the distance already fits the frame
        }
        else if (walker.Seated)
        {
            Vector3 f = Flat(walker.transform.forward);
            aim = walker.transform.position + Vector3.up * (HeadHeight * 0.65f);
            origin = aim;
            offset = f * seatFront + Vector3.up * seatUp +
                     Vector3.Cross(Vector3.up, f).normalized * seatSide;
            wantFov = seatFov;
        }
        else if (walker.CurrentStone != null && !holdShotAtStones)
        {
            // Deliberately NOT swung round to face the stone. Re-composing on every
            // word turns the camera a different way at each stop, and the library
            // behind them jumps across the frame. The word is nudged into shot by
            // biasing the aim a little, and that is all.
            Vector3 word = walker.CurrentStone.position + Vector3.up *
                           (0.55f * Mathf.Max(0.2f, walker.CurrentStone.lossyScale.y));
            aim = Vector3.Lerp(head, word, wordBias);
            origin = aim;

            Vector3 readOffset = -fwd * readBack + Vector3.up * readUp +
                                 lateral * (bank * readSide);
            // With the heading held, the read shot may come CLOSER but must not
            // come from a NEW ANGLE: travel and read were authored as separate
            // offsets, and swapping between them tilts the lens every time the
            // reader reaches a stone — which looks exactly like the world turning,
            // even though nothing rotated. Same direction, shorter reach.
            offset = lockWalkDirection
                ? travelOffset.normalized * readOffset.magnitude
                : readOffset;
            wantFov = readFov;
        }
        else
        {
            aim = head + fwd * lookAhead - Vector3.up * 0.25f;
            origin = head;
            offset = travelOffset;
            wantFov = travelFov;
        }

        float pull = 1f;
        if (!solved) wantFov = SolveShot(wantFov, out pull);
        Vector3 wantPos = origin + offset * pull;

        // The floor. The shot maths can put the lens anywhere; this is the promise
        // that it is never in the reader's face — back off along the line the shot
        // already chose, so the angle it composed survives.
        // The book shot is exempt: it is framed on the book, and the reader walks
        // right up to that, so backing off to clear THEM throws away the framing.
        Vector3 fromReader = wantPos - head;
        float near = focus != null ? 0f : Mathf.Max(0.1f, minReaderDistance);
        if (near > 0f && fromReader.sqrMagnitude < near * near &&
            fromReader.sqrMagnitude > 1e-4f)
            wantPos = head + fromReader.normalized * near;

        // INDOORS THE CAMERA GOES IN WITH THEM, AND TURNS WITH THEM.
        //
        // Two things are wrong with the outdoor shot in a room. It is composed for
        // open water — six metres back, four up — and that boom is longer than the
        // library is wide, so the lens ends up beyond a wall watching the outside
        // of the building. And it is built on a LOCKED heading (the river's), which
        // is right on the river and useless inside: the reader turns to look at a
        // shelf and the camera keeps facing the water, so you never see what they
        // are looking at.
        //
        // So indoors the shot is rebuilt on the READER'S OWN FACING, close behind
        // them — which is exactly what "show me where they are looking" means. It
        // is blended in by `indoors`, so the change happens over a stride at the
        // door rather than as a cut.
        float indoors = Insideness(walker.transform.position);
        IndoorsBlend = indoors;          // read by the report; 0 outside, 1 inside

        // WHAT THE SHOT IS ABOUT MAY BE INDOORS EVEN WHEN THE READER IS NOT. The
        // reveal frames the middle of the hall while the reader is still on the
        // threshold; the book shot frames a page on a desk at the back of the room
        // while they walk in. In both the subject is in the library, so the lens
        // belongs in the library — otherwise it solves its distance in open air and
        // ends up outside the front wall, filming the building it is supposed to be
        // inside. This is the reading that closes the walls for the camera.
        Vector3 subject = focus != null ? focus.position : aim;

        // The walls get their own reading, on a ramp no longer than a stride — see
        // WallsShut. The shot may ease indoors over whatever length reads well; the
        // building may not be half there while it does.
        float wallsShut = Mathf.Max(WallsShut(walker.transform.position),
                                    WallsShut(subject));
        ShotInsideBlend = wallsShut;

        if (indoors > 0f && focus == null)
        {
            // THROUGH THEIR EYES — but only while there is nothing else to look at.
            // Any shot that looks AT the reader shows you the back of their head
            // instead of the room, which is the opposite of what a look around a
            // library is for. At the eyes, facing the way they face, turning the
            // reader turns the view — so what they are looking at is simply what
            // you see.
            //
            // With a focus set this stands down: the book shot exists to make one
            // small page readable, and replacing it with a horizontal view from the
            // reader's own eyes puts that page at the very bottom of the frame.
            // Reading is the game; the room is the place it happens in.
            Vector3 face = Flat(walker.transform.forward);
            Vector3 eye = walker.transform.position + Vector3.up * EyeHeight
                        + face * eyeForward;

            wantPos = Vector3.Lerp(wantPos, eye, indoors);
            aim = Vector3.Lerp(aim, eye + face * 3f, indoors);
            wantFov = Mathf.Lerp(wantFov, IndoorFov(), indoors);
        }

        // Never film the sea. minHeight is an absolute floor and does nothing once
        // the reader climbs; this keeps the lens above THEM as well, which is what
        // stops the shot sinking to water level and filling the frame with ocean.
        wantPos.y = Mathf.Max(wantPos.y, minHeight,
                              walker.transform.position.y + minAboveReader);

        // Both of the guards below exist to protect a shot of the reader, and in
        // first person there is no such shot to protect — the reader is behind the
        // lens. Left running they would undo it: the clearance test would shove the
        // camera toward a point three metres ahead (into whatever is being looked
        // at), and the framing guard would swing the view back round onto the
        // reader's own head. So they stand down as the eye view fades in.
        if (indoors < 0.5f)
        {
            // Nothing solid between the subject and the lens — on the climb the
            // shot wants a position inside the hill, which renders as a wall of
            // terrain with nobody in it.
            wantPos = Unobstructed(aim, wantPos);

            // Whatever the shot wanted, the reader stays in frame.
            aim = KeepReaderInFrame(wantPos, aim, head);
        }

        // THE LAST WORD, AFTER EVERY OTHER RULE HAS HAD ITS SAY. You cannot stand
        // in a house and look at the outside of it. Every shot above solves its
        // distance in open air — the reveal asks for five metres back from the
        // middle of a room four metres deep — so without this the lens walks
        // straight through the front wall and the "arrival at the library" is a
        // picture of the island. It runs last on purpose: the height floor and the
        // clearance test can both push the lens back out, and a wall that anything
        // may overrule is not a wall.
        wantPos = InsideTheHall(wantPos, wallsShut);

        aimTarget.position = aim;
        if (!driveCamera) { ShowReader(true); return; }

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

        // Tested against where the lens ACTUALLY ended up, not where the shot asked
        // to be: damping means those differ for half a second at the door, and that
        // half second is exactly when the lens is passing through the reader.
        ShowReader(!InsideTheReader(transform.position));
    }

    /// <summary>
    /// Is the lens inside the reader's own bulk? Generous — shoulders, head and the
    /// peak of the cap — because a body clipping the near plane is far worse than a
    /// body hidden a few centimetres early.
    /// </summary>
    bool InsideTheReader(Vector3 lens)
    {
        if (!hideReaderInFirstPerson || walker == null) return false;
        float h = ReaderHeight > 0.2f ? ReaderHeight : eyeHeight;
        Vector3 head = walker.transform.position + Vector3.up * (h * 0.9f);
        float r = h * 0.42f;
        return (lens - head).sqrMagnitude < r * r;
    }

    bool _readerShown = true;
    PlaceholderActor _actor;
    bool _actorLooked;

    /// <summary>Show or hide the reader's body. Cheap to call every frame.</summary>
    void ShowReader(bool show)
    {
        if (walker == null) return;

        if (!_actorLooked)
        {
            _actor = walker.GetComponentInChildren<PlaceholderActor>(true);
            _actorLooked = true;
        }
        if (_actor != null) { _actor.SetVisible(show); _readerShown = show; return; }

        if (show == _readerShown) return;       // a real model: only touch on change
        _readerShown = show;
        foreach (var r in Renderers(walker.transform))
            if (r != null) r.enabled = show;
    }

    /// <summary>
    /// The lens that makes the book fill <see cref="focusFill"/> of the frame from
    /// <see cref="focusDistance"/> away.
    ///
    /// The walk shots are authored as offsets in metres and corrected for the frame
    /// afterwards (see SolveShot). That is the wrong way round for a shot whose only
    /// job is to make one small object readable — and correcting it by MOVING is
    /// worse still here: a portrait frame shows about four times less world sideways,
    /// so fitting by distance reverses the camera several metres inland, and the book
    /// stands at the edge of the island with a hill right behind it.
    ///
    /// So the lens moves and the camera does not. The tight axis wins: on portrait
    /// that is the horizontal one, and fitting to the vertical would push the book
    /// off the sides.
    /// </summary>
    float SolveFocus(out float distance)
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        float aspect = _cam != null && _cam.aspect > 0.01f ? _cam.aspect : referenceAspect;
        float radius = FocusRadius();
        float fill = Mathf.Clamp(focusFill, 0.05f, 0.95f);
        float baseDist = Mathf.Max(0.4f, focusDistance);
        float lo = Mathf.Min(focusMinFov, focusMaxFov);
        float hi = Mathf.Max(focusMinFov, focusMaxFov);

        // The lens that fills the tight axis from the authored distance…
        float half = Mathf.Atan(radius / Mathf.Max(0.05f, fill * baseDist));
        float vHalf = aspect < 1f ? Mathf.Atan(Mathf.Tan(half) / aspect) : half;
        float fov = 2f * vHalf * Mathf.Rad2Deg;

        distance = baseDist;
        if (fov <= hi) return Mathf.Max(fov, lo);

        // …and when that lens would be wider than we allow, the camera takes over
        // for the rest. This is the difference between Focus Fill being a real knob
        // and being decorative: pinned at the FOV ceiling, turning it down changes
        // nothing, because the frame simply cannot open any further.
        fov = hi;
        float vCap = fov * 0.5f * Mathf.Deg2Rad;
        float tight = aspect < 1f ? Mathf.Atan(Mathf.Tan(vCap) * aspect) : vCap;
        float want = radius / Mathf.Max(1e-4f, fill * Mathf.Tan(tight));

        distance = Mathf.Clamp(want, baseDist,
                               baseDist * Mathf.Max(1f, focusMaxPullback));
        return fov;
    }

    static readonly RaycastHit[] _blockers = new RaycastHit[24];

    /// <summary>
    /// Keep the line from the subject to the lens clear, so the camera stops short
    /// of the island instead of ending up inside it looking at the inside of a hill.
    ///
    /// TWO THINGS ARE DELIBERATELY NOT OBSTACLES.
    ///   • Triggers — the clickable page text is made of trigger colliders and must
    ///     never push the camera around.
    ///   • The word stones and the reader — on the river the stones sit BETWEEN the
    ///     lens and the reader on purpose, and the reader is the subject. Treating
    ///     either as a wall drags the camera into the reader's back at every stone,
    ///     which is worse than the problem this solves.
    /// </summary>
    Vector3 Unobstructed(Vector3 from, Vector3 to)
    {
        Vector3 d = to - from;
        float len = d.magnitude;
        if (len < 0.05f) return to;
        d /= len;

        int n = Physics.SphereCastNonAlloc(from, Mathf.Max(0.02f, collisionRadius), d,
                                           _blockers, len, collideWith,
                                           QueryTriggerInteraction.Ignore);
        float nearest = len;
        for (int i = 0; i < n; i++)
        {
            var h = _blockers[i];
            if (h.distance <= 0.0001f) continue;          // started inside it
            if (Ignored(h.collider)) continue;
            if (h.distance < nearest) nearest = h.distance;
        }
        if (nearest >= len) return to;
        return from + d * Mathf.Max(0.45f, nearest - collisionBuffer);
    }

    Bounds _hall;
    bool _hallFound;

    /// <summary>
    /// How far into the library the reader is, as the camera sees it. 0 = the eye
    /// view is off entirely. Exposed because "the camera is still outside" has
    /// exactly two causes — this reading 0 when it should not, or the shot itself
    /// being wrong — and they need opposite fixes.
    /// </summary>
    public float IndoorsBlend { get; private set; }

    /// <summary>
    /// How much the SHOT is an interior — the reader inside, or the thing being
    /// framed inside. This, not <see cref="IndoorsBlend"/>, is what shuts the
    /// camera in with the walls.
    /// </summary>
    public float ShotInsideBlend { get; private set; }

    /// <summary>
    /// Metres the last frame's shot had to be pushed back into the room. Anything
    /// above zero means a shot asked to be outside the building and was refused —
    /// which is worth knowing, because the honest fix is usually to compose that
    /// shot for the room rather than to lean on the clamp.
    /// </summary>
    public float PushedBackIn { get; private set; }

    /// <summary>The hall the camera measured, for the report. Size zero = not found.</summary>
    public Bounds HallBounds => _hallFound ? _hall : new Bounds();

    /// <summary>The world height of the ceiling the camera is keeping under.</summary>
    public float HallCeilingY => _hallFound ? _hall.max.y + Mathf.Max(0.5f, hallHeight) : 0f;

    /// <summary>The room's plan, measured off the floor tiles. False = no library.</summary>
    bool Hall()
    {
        if (_hallFound) return true;

        LibraryFloor.TilePrefix = hallFloorPrefix;
        if (!LibraryFloor.Known) return false;
        _hall = LibraryFloor.Plan;
        _hallFound = true;
        return true;
    }

    /// <summary>Forget the measured room — for the editor, or a rebuilt world.</summary>
    public void ResetHall() { _hallFound = false; LibraryFloor.Forget(); }

    /// <summary>
    /// 0 outside the library, 1 well inside, eased across the threshold.
    ///
    /// Measured against the LIBRARY FLOOR, not by raycasting for a roof. The
    /// imported world carries no colliders on its scenery, so a probe upward finds
    /// nothing and reports "outdoors" while the reader stands in the middle of the
    /// hall. The floor tiles are right there in the scene with real bounds, and
    /// they describe the room exactly.
    ///
    /// Eased rather than switched: crossing the door should draw the camera in over
    /// a stride, not cut.
    /// </summary>
    float Insideness(Vector3 p) => Insideness(p, ThresholdBlend);

    float Insideness(Vector3 p, float blend)
    {
        if (!Hall()) return 0f;

        // Height matters as well as footprint: the river passes under the island's
        // edge, and without this the reader would read as "indoors" from the water.
        if (p.y < _hall.min.y - 1f || p.y > HallCeilingY + 1f) return 0f;

        float dx = Mathf.Abs(p.x - _hall.center.x) - _hall.extents.x;
        float dz = Mathf.Abs(p.z - _hall.center.z) - _hall.extents.z;
        float outside = Mathf.Max(dx, dz);            // negative once inside
        return Mathf.Clamp01(-outside / Mathf.Max(0.1f, blend));
    }

    /// <summary>
    /// The threshold blend, held to a stride.
    ///
    /// This ramp is measured from the NEAREST WALL, so its length is also the width
    /// of the band around the room in which the reader counts as only half indoors.
    /// Set it to 1.2 m in a hall 4.2 m deep and there is no standing position left
    /// that reads as inside at all: the camera spends the entire visit half way
    /// between an eye and a six-metre boom — a position that is neither, and is
    /// usually in the masonry. A door is a stride deep, so the blend is one.
    /// </summary>
    float ThresholdBlend => Mathf.Clamp(thresholdBlend, 0.1f, 0.6f);

    /// <summary>
    /// The same test, for the WALLS rather than for the shot.
    ///
    /// A wall is not 62% solid because the reader happens to be standing near one.
    /// Whatever length the threshold ramp is tuned to for the sake of the eye view,
    /// the room closes around the lens within a stride of the door — otherwise a
    /// clamp that is only partly applied leaves the camera partly outside, which is
    /// the whole complaint.
    /// </summary>
    float WallsShut(Vector3 p) => Insideness(p, Mathf.Min(0.3f, ThresholdBlend));

    /// <summary>
    /// Hold the lens inside the room, off the walls and under the ceiling.
    ///
    /// The room's PLAN is the floor tiles' own bounds, which stop a little short of
    /// the walls — so clamping to them and then insetting by
    /// <see cref="wallClearance"/> leaves the lens comfortably in the masonry's lee
    /// rather than in it. The HEIGHT cannot be measured the same way (nothing in
    /// the scene is the ceiling), so it is <see cref="hallHeight"/> above the floor.
    ///
    /// Blended by <paramref name="inside"/>, so walking through the door draws the
    /// camera in over a stride instead of snapping it.
    /// </summary>
    Vector3 InsideTheHall(Vector3 pos, float inside)
    {
        PushedBackIn = 0f;
        if (!stayInsideTheHall || inside <= 0.001f || !Hall()) return pos;

        float pad = Mathf.Max(0f, wallClearance);
        // A room smaller than twice the clearance would invert; meet in the middle.
        float hx = Mathf.Max(0.2f, _hall.extents.x - pad);
        float hz = Mathf.Max(0.2f, _hall.extents.z - pad);

        var shut = new Vector3(
            Mathf.Clamp(pos.x, _hall.center.x - hx, _hall.center.x + hx),
            Mathf.Clamp(pos.y, _hall.max.y + 0.4f,
                        Mathf.Max(_hall.max.y + 0.6f,
                                  HallCeilingY - Mathf.Max(0f, ceilingClearance))),
            Mathf.Clamp(pos.z, _hall.center.z - hz, _hall.center.z + hz));

        PushedBackIn = Vector3.Distance(pos, shut) * inside;
        return Vector3.Lerp(pos, shut, inside);
    }

    /// <summary>
    /// The indoor lens, solved for the frame we actually have.
    ///
    /// A field of view is authored as the VERTICAL angle, and on a portrait phone
    /// that is the roomy axis — the narrow one is across, where 62° vertical on a
    /// 720x1520 screen leaves 32°. Standing in a five-metre hall and seeing 32° of
    /// it is why an interior can read as a corridor. So indoors the horizontal
    /// field is the thing asked for and the vertical is worked out from it.
    /// </summary>
    float IndoorFov()
    {
        if (_cam == null) _cam = GetComponent<Camera>();
        float aspect = _cam != null && _cam.aspect > 0.01f ? _cam.aspect : referenceAspect;
        if (indoorHorizontalFov <= 1f) return indoorFov;

        float hHalf = Mathf.Clamp(indoorHorizontalFov, 10f, 160f) * 0.5f * Mathf.Deg2Rad;
        float v = 2f * Mathf.Atan(Mathf.Tan(hHalf) / Mathf.Max(0.05f, aspect)) * Mathf.Rad2Deg;
        return Mathf.Clamp(v, indoorFov, Mathf.Max(indoorFov, indoorMaxFov));
    }

    /// <summary>The subject and the words are never obstacles — see Unobstructed.</summary>
    bool Ignored(Collider c)
    {
        if (c == null) return true;
        if (c.GetComponentInParent<WordStone>() != null) return true;
        if (c.GetComponentInParent<StoneSlot>() != null) return true;
        if (walker != null && c.transform.IsChildOf(walker.transform)) return true;
        if (focus != null && c.transform.IsChildOf(focus)) return true;
        return false;
    }

    /// <summary>
    /// Pull the aim back toward the reader until they are within
    /// <see cref="maxOffCentre"/> of the middle of the frame.
    ///
    /// The travel shot leads down the road so you can see what is coming, which is
    /// right — but on the climb the road bends away and that lead walks the frame
    /// off the reader entirely, leaving a camera touring scenery with nobody in it.
    /// This caps how far it may lead: it still looks ahead, never past them.
    /// </summary>
    Vector3 KeepReaderInFrame(Vector3 camPos, Vector3 aim, Vector3 head)
    {
        if (maxOffCentre <= 0f) return aim;

        Vector3 toAim = aim - camPos, toHead = head - camPos;
        if (toAim.sqrMagnitude < 1e-6f || toHead.sqrMagnitude < 1e-6f) return aim;

        float off = Vector3.Angle(toAim, toHead);
        if (off <= maxOffCentre) return aim;

        Vector3 dir = Vector3.RotateTowards(toHead.normalized, toAim.normalized,
                                            maxOffCentre * Mathf.Deg2Rad, 0f);
        return camPos + dir * toAim.magnitude;
    }

    Transform _measured;
    float _measuredRadius = 0.5f;

    /// <summary>
    /// The framed object's radius, measured once per target. Cached deliberately:
    /// the page text is spawned into the book at runtime and its glyph bounds would
    /// otherwise keep changing the answer — and with it the shot — every time a
    /// sentence is laid out.
    /// </summary>
    float FocusRadius()
    {
        if (focusRadius > 0.001f) return focusRadius;
        if (focus == _measured) return _measuredRadius;

        _measured = focus;
        _measuredRadius = 0.5f;
        if (focus == null) return _measuredRadius;

        // The aim point is usually an empty marker inside the model, so fall back
        // to its parent — that is where the mesh actually lives.
        var rends = Renderers(focus);
        bool fromParent = false;
        if (rends.Length == 0 && focus.parent != null)
        {
            rends = Renderers(focus.parent);
            fromParent = rends.Length > 0;
        }
        if (rends.Length == 0) return _measuredRadius;

        var b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        _measuredRadius = Mathf.Max(0.05f, b.extents.magnitude);

        // THE PARENT OF A SOCKET IN THE HALL IS THE WHOLE BUILDING. SOCKET_HallCentre
        // hangs off SM_Library_Exterior, so the fallback above measures the library
        // from its porch steps to its roof finial — seven metres of radius for a
        // marker standing in a room four metres deep. Ask a shot to fit that and it
        // solves to the far side of the front wall every time, which is precisely
        // how "the reveal of the hall" became a picture of the island. When the
        // point being framed is INSIDE the room, the room is what we are framing.
        if (fromParent && Hall() && Insideness(focus.position) > 0.5f)
        {
            float roomRadius = new Vector3(_hall.extents.x,
                                           Mathf.Max(0.5f, hallHeight) * 0.5f,
                                           _hall.extents.z).magnitude;
            _measuredRadius = Mathf.Min(_measuredRadius, roomRadius);
        }
        return _measuredRadius;
    }

    static Renderer[] Renderers(Transform t) =>
        System.Array.FindAll(t.GetComponentsInChildren<Renderer>(),
                             r => !(r is ParticleSystemRenderer));

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
