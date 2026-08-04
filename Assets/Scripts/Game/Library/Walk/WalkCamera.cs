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
        b.Encapsulate(walker.transform.position + Vector3.up * headHeight);

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
        Vector3 reader = walker.transform.position + Vector3.up * (headHeight * 0.6f);
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

        Vector3 head = walker.transform.position + Vector3.up * headHeight;
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
            aim = walker.transform.position + Vector3.up * (headHeight * 0.65f);
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

        // Never film the sea. minHeight is an absolute floor and does nothing once
        // the reader climbs; this keeps the lens above THEM as well, which is what
        // stops the shot sinking to water level and filling the frame with ocean.
        wantPos.y = Mathf.Max(wantPos.y, minHeight,
                              walker.transform.position.y + minAboveReader);

        // Nothing solid between the subject and the lens — on the climb the shot
        // wants a position inside the hill, which renders as a wall of terrain with
        // nobody in it. This used to run for the book shot only; the walk needs it
        // more, because that is where the island gets between them.
        wantPos = Unobstructed(aim, wantPos);

        // Whatever the shot wanted, the reader stays in frame.
        aim = KeepReaderInFrame(wantPos, aim, head);

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
        if (rends.Length == 0 && focus.parent != null) rends = Renderers(focus.parent);
        if (rends.Length == 0) return _measuredRadius;

        var b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        _measuredRadius = Mathf.Max(0.05f, b.extents.magnitude);
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
