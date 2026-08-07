// ===========================================================================
//  PathWalker — walks the road, stops at every word, climbs, sits on the mat
// ===========================================================================
//  One coroutine, one number: how far along the road we are. Everything else
//  (position, facing, which stone we are beside, what the camera should frame)
//  falls out of that.
//
//      walk in → stop at word 1 → dwell → walk → … → last word
//              → climb the island → sit on the mat
//
//  It reports what it is doing through three events, so the reading loop can
//  be hung off it later WITHOUT changing this file:
//      onArrive(i)  — standing beside stone i, facing it. Open the challenge here.
//      onLeave(i)   — moving off toward stone i+1.
//      onSeated     — the reader is on the mat; the passage is done.
//
//  While `waitForKeyStones` is off it is a self-playing demo: it dwells for a
//  beat at each word and walks on. Turn it on and a gold stone holds the walk
//  until its challenge is solved — which is the shape the real game wants.
//
//  MVVM note: this is a presenter. It moves a transform and raises events. It
//  never scores, saves or asks a service anything.
// ===========================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PathWalker : MonoBehaviour
{
    [Header("Wiring")]
    public RiversideRoad road;
    [Tooltip("Optional. Only used to check the river is laid out in world space — " +
             "a walking camera cannot use the fixed-camera perspective trick.")]
    public WordPathBuilder path;
    [Tooltip("Optional. The placeholder body. Any model works — this is only asked " +
             "to stride, and to fold up when seated.")]
    public PlaceholderActor actor;

    [Header("Pace")]
    [Tooltip("Metres per second. A child reading along wants this SLOW.")]
    public float walkSpeed = 1.15f;
    [Tooltip("How long the reader stands at each word before moving on.")]
    public float dwellSeconds = 1.8f;
    [Tooltip("Degrees per second the reader turns.")]
    public float turnSpeed = 320f;
    [Tooltip("How far before the first word the walk begins, so it arrives moving.")]
    public float approach = 1.6f;
    [Tooltip("Metres per second for walks that are NOT the reading walk — going to " +
             "the book, coming back from the mat. Brisker: nobody wants to watch " +
             "the reader stroll back to the lectern at reading pace.")]
    public float transitSpeed = 2.4f;

    [Header("Player control")]
    [Tooltip("Let the player drive instead of playing the walk automatically. " +
             "CLICK A STONE (or anywhere along the river) and the reader walks " +
             "there and stops — no holding the button down. Arrow keys or W/S " +
             "nudge them along for fine control.")]
    public bool followPointer = true;
    [Tooltip("Clicking within this distance of a word lands the reader exactly ON " +
             "it rather than near it, so you never stop half a step short.")]
    public float snapToStone = 1.2f;
    [Tooltip("How near a word counts as standing at it, in metres. This is what " +
             "fires onArrive / onLeave when the walk is driven by hand.")]
    public float arriveRadius = 0.6f;
    [Tooltip("How fast the arrow keys / W / S move the destination, as a multiple " +
             "of walk speed.")]
    public float keyNudgeRate = 1.5f;

    [Header("Free walk inside the library")]
    [Tooltip("INSIDE THE HALL THE ROAD LETS GO. Everywhere else the reader is a " +
             "distance along a line, which is right — the river IS a line, and a " +
             "reader who can wander off it can wander away from the words. A room " +
             "is not a line. On the library floor the same two inputs mean what " +
             "they mean in a room: forward walks the way the reader is facing, and " +
             "left/right TURNS them, so standing in the middle and looking round is " +
             "how you see the shelves, the desk, the globe and the lanterns.")]
    public bool roamIndoors = true;
    [Tooltip("Walking pace indoors, in metres per second. Slower than the transit " +
             "walk: this is looking around, not going somewhere.")]
    public float roamSpeed = 1.25f;
    [Tooltip("Degrees per second the reader turns on the spot indoors.")]
    public float roamTurnSpeed = 115f;
    [Tooltip("How far off the walls the reader is held, in metres — their own " +
             "shoulder width, near enough.")]
    public float roamEdge = 0.5f;
    [Tooltip("How far off the furniture they are held. The desk, the globe and the " +
             "book stack are solid; walking through them is what makes a room read " +
             "as a picture of a room.")]
    public float roamClearance = 0.35f;
    [Tooltip("Name prefixes of the things in the hall that are solid.")]
    public string[] roamObstacles =
        { "Desk_Reading", "Chair", "Globe", "BookStack", "Scrolls", "Column" };
    [Tooltip("The doorway — SOCKET_Entrance. The one gap in the walls: walk out " +
             "through it and the road takes the reader back again. Left empty it is " +
             "taken from the BookStation, and without one the library has no way out.")]
    public Transform libraryDoor;
    [Tooltip("How wide the doorway is for walking through, in metres.")]
    public float roamDoorWidth = 1.8f;

    [Header("Behaviour")]
    public bool autoPlay = true;
    [Tooltip("Hold at a gold key stone until its challenge is solved, instead of " +
             "dwelling for a fixed beat. This is the real game's rule.")]
    public bool waitForKeyStones = false;
    [Tooltip("Tapping the word the reader is standing at sends them on early.")]
    public bool advanceOnTap = true;
    [Tooltip("Print entering and leaving the free walk to the Console.")]
    public bool logRoam = true;

    [Header("Events")]
    public StoneEvent onArrive = new StoneEvent();
    public StoneEvent onLeave = new StoneEvent();
    public UnityEvent onSeated = new UnityEvent();

    [System.Serializable] public class StoneEvent : UnityEvent<int> { }

    /// <summary>How far along the road the reader has walked, in metres.</summary>
    public float Distance { get; private set; }

    /// <summary>The stone being read right now — null while walking.</summary>
    public Transform CurrentStone { get; private set; }

    /// <summary>Index of that stone, or -1.</summary>
    public int CurrentIndex { get; private set; } = -1;

    /// <summary>True once the reader is on the mat.</summary>
    public bool Seated { get; private set; }

    /// <summary>
    /// Which way the reader is heading — the camera leads with this. Off the road
    /// (walking to the book) the road's tangent is meaningless, so the body's own
    /// facing is the answer.
    /// </summary>
    public Vector3 Heading =>
        !_offRoad && road != null ? road.TangentAt(Distance) : transform.forward;

    bool _skipDwell;
    bool _offRoad;
    bool _scripted;                 // a coroutine is walking them; hands off
    Coroutine _run;

    /// <summary>
    /// ALWAYS place the reader, whatever is going to move them afterwards.
    ///
    /// Begin() does two separate jobs — it stands the reader on the road, and it
    /// starts the automatic walk — and this used to skip both when autoPlay was
    /// off. Turning autoPlay off to hand control to the compass therefore left the
    /// reader unplaced, sitting wherever the scene last saved them, with the camera
    /// faithfully following them inside the library wall. Standing them on the road
    /// is not optional; only walking them along it is.
    /// </summary>
    void Start() { Begin(); }

    /// <summary>Start (or restart) the walk from the first word.</summary>
    public void Begin()
    {
        if (road == null) road = FindAnyObjectByType<RiversideRoad>();
        if (road == null) { Debug.LogError("[Walk] No RiversideRoad in the scene."); return; }

        GuardLayout();
        if (road.StopCount == 0) road.Rebuild();
        if (road.StopCount == 0)
        {
            Debug.LogWarning("[Walk] The road has no stops — the sentence has not " +
                             "been laid out yet. Build the word path first.");
            return;
        }

        if (_run != null) StopCoroutine(_run);
        _offRoad = false;
        Roaming = false;
        Seated = false;
        if (actor != null) actor.SetSeated(false);
        Distance = Mathf.Max(0f, road.StopDistance(0) - Mathf.Max(0f, approach));
        _goal = Distance;          // stand still until the player says otherwise
        Place(Distance, snapFacing: true);

        // The reader is now standing on the road. Whether anything WALKS them from
        // here is a separate question: under the finger or the compass there is no
        // script to run — Update() drives it, and arriving at a word is something
        // that happens when you get near one rather than something a coroutine
        // decides for you.
        if (!followPointer && autoPlay) _run = StartCoroutine(Walk());
    }

    /// <summary>Send the reader on now, without waiting out the rest of the dwell.</summary>
    public void Continue() { _skipDwell = true; }

    /// <summary>
    /// Stop whatever road walk is running and leave the reader standing where they
    /// are. The reading loop calls this before it takes the reader off-road.
    /// </summary>
    public void Halt()
    {
        if (_run != null) StopCoroutine(_run);
        _run = null;
        CurrentStone = null;
        CurrentIndex = -1;
    }

    /// <summary>
    /// Walk in a straight line to a world point, OFF the road. The road only knows
    /// the river; the book stands beside it, so fetching the reader to the lectern
    /// and sending them back is a plain point-to-point walk. Yield on this from the
    /// conductor.
    /// </summary>
    /// <param name="speed">Metres per second; 0 uses <see cref="transitSpeed"/>.</param>
    /// <param name="face">Where to look once arrived. Null = keep the arrival heading.</param>
    public IEnumerator WalkTo(Vector3 point, float speed = 0f, Vector3? face = null)
    {
        Halt();
        _offRoad = true;
        Seated = false;
        if (actor != null) actor.SetSeated(false);

        // A scripted walk owns the reader for its whole length. Without this the
        // free walk indoors would be reading the compass at the same time, and the
        // reader would be pulled two ways down the middle of the hall.
        _scripted = true;
        Roaming = false;
        try
        {
            Vector3 start = transform.position;
            float total = Vector3.Distance(start, point);
            float v = speed > 0.01f ? speed
                    : transitSpeed > 0.01f ? transitSpeed : walkSpeed;

            if (total > 0.05f)
            {
                Vector3 heading = Flat(point - start);
                float gone = 0f;
                while (gone < total)
                {
                    float step = Mathf.Min(Mathf.Max(0.05f, v) * Time.deltaTime, total - gone);
                    gone += step;
                    transform.position = Vector3.Lerp(start, point, gone / total);
                    if (heading.sqrMagnitude > 1e-4f)
                        transform.rotation = Quaternion.RotateTowards(
                            transform.rotation,
                            Quaternion.LookRotation(heading, Vector3.up),
                            turnSpeed * Time.deltaTime);
                    if (actor != null) actor.Step(step);
                    yield return null;
                }
            }

            transform.position = point;
            yield return Turn(face);
        }
        finally { _scripted = false; }
    }

    /// <summary>
    /// Get up off the mat. Seated stops every input dead — which is right while the
    /// reader is sitting, and wrong the moment anything wants them to walk again.
    /// </summary>
    public void Stand()
    {
        Seated = false;
        if (actor != null) actor.SetSeated(false);
    }

    /// <summary>Turn on the spot to face a direction. Null or zero = do nothing.</summary>
    public IEnumerator Turn(Vector3? face)
    {
        if (!face.HasValue) yield break;
        Vector3 f = Flat(face.Value);
        if (f.sqrMagnitude < 1e-4f) yield break;

        var want = Quaternion.LookRotation(f, Vector3.up);
        while (Quaternion.Angle(transform.rotation, want) > 1.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, want, turnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = want;
    }

    /// <summary>
    /// The walking camera makes the river a WORLD object again: nothing has to be
    /// squeezed into one still frame any more. If the fixed-camera compensation is
    /// still on, it re-solves the stone positions every time the camera moves and
    /// the reader ends up walking past stones that are sliding about. Turn it off.
    /// </summary>
    void GuardLayout()
    {
        if (path == null) path = FindAnyObjectByType<WordPathBuilder>();
        if (path == null) return;
        if (path.perspectiveCompensation <= 0.001f && !path.frameOnScreen) return;

        Debug.Log("[Walk] Switching the river to world-space layout — a camera that " +
                  "follows the reader replaces the fixed-camera perspective fix.");
        path.perspectiveCompensation = 0f;
        path.frameOnScreen = false;
        path.stoneScreenWidth = 0f;
        path.minFitFactor = 1f;
        path.ApplyPerspective();
        road.Rebuild();
    }

    // ── walking by hand ─────────────────────────────────────────────────────

    /// <summary>
    /// Where along the road the reader is currently heading. The whole of player
    /// control is "set this number"; the walking itself never changes.
    /// </summary>
    float _goal;

    /// <summary>
    /// Held each frame by the on-screen compass: -1 back, +1 onward. Added to the
    /// keyboard, so the pad and the keys drive the SAME walk rather than the pad
    /// introducing a second way of moving.
    /// </summary>
    [System.NonSerialized] public float driveForward;

    /// <summary>
    /// Held each frame by the compass: -1 turn left, +1 turn right. Turning happens
    /// ON THE SPOT and only while standing still — the reader is looking around,
    /// not steering. Walking still faces them down the road, so this can never put
    /// them at odds with the path.
    /// </summary>
    [System.NonSerialized] public float driveTurn;

    /// <summary>True while the compass is asking for anything at all.</summary>
    public bool BeingDriven => Mathf.Abs(driveForward) > 0.01f ||
                               Mathf.Abs(driveTurn) > 0.01f;

    /// <summary>
    /// Drive the walk by hand. ONE destination, set by a click and nudged by keys,
    /// and the reader always walks toward it at their own pace.
    ///
    /// This replaces hold-to-move, which wanted the button held down AND the cursor
    /// kept over a thin, obliquely-viewed road — fine with a finger on a phone,
    /// miserable with a trackpad. Click once and let go: the reader goes there and
    /// stops. They still cannot be dragged off the path or teleported, because the
    /// click only ever chooses a DISTANCE ALONG it.
    /// </summary>
    void Update()
    {
        if (Seated) return;

        // The floor of the library is not a line, so while they are standing on it
        // nothing below this runs — see Roam.
        if (roamIndoors && !_scripted && LibraryFloor.Contains(transform.position))
        {
            if (!Roaming) EnterRoam();
            Roam();
            return;
        }
        if (Roaming) LeaveRoam();

        if (road == null || road.StopCount == 0) return;
        if (!followPointer && !BeingDriven) return;

        if (followPointer && PointerPressed(out Vector2 screen)) AimAt(screen);

        // Turning is a standing-still thing: while the reader is walking, Place()
        // faces them down the road, so a turn applied at the same time would just
        // be overwritten a line later and feel broken.
        float forward = Mathf.Clamp(KeyAxis() + driveForward, -1f, 1f);
        if (Mathf.Abs(driveTurn) > 0.01f && Mathf.Abs(forward) < 0.01f)
        {
            transform.Rotate(0f, driveTurn * turnSpeed * 0.35f * Time.deltaTime, 0f);
            return;
        }

        if (Mathf.Abs(forward) > 0.01f)
            _goal = Mathf.Clamp(_goal + forward * walkSpeed * keyNudgeRate * Time.deltaTime,
                                0f, road.Length);

        float delta = _goal - Distance;
        if (Mathf.Abs(delta) < 0.01f) return;

        float step = Mathf.Min(walkSpeed * Time.deltaTime, Mathf.Abs(delta));
        Distance = Mathf.Clamp(Distance + step * Mathf.Sign(delta), 0f, road.Length);
        Place(Distance, snapFacing: false);
        if (actor != null) actor.Step(step);

        NoteWhereWeAre();
    }

    /// <summary>Turn a click into a destination along the road.</summary>
    void AimAt(Vector2 screen)
    {
        var cam = Camera.main;
        if (cam == null) return;
        Ray ray = cam.ScreenPointToRay(screen);

        // A word you clicked is a word you meant. Going to the stone's own stop
        // beats going to the nearest bit of road, which can leave the reader
        // standing just past it with the label behind their shoulder.
        if (Physics.Raycast(ray, out var hit, 500f, ~0, QueryTriggerInteraction.Ignore))
        {
            var ws = hit.collider.GetComponentInParent<WordStone>();
            if (ws != null)
            {
                for (int i = 0; i < road.StopCount; i++)
                    if (road.Stone(i) == ws.transform) { _goal = road.StopDistance(i); return; }
            }
        }

        // Otherwise: anywhere on the water will do. A level plane through the
        // reader's feet, not colliders, so clicking open sea still means something.
        var ground = new Plane(Vector3.up, transform.position);
        if (!ground.Raycast(ray, out float enter)) return;

        float d = road.NearestDistance(ray.GetPoint(enter));

        // land ON a word rather than a step short of it
        for (int i = 0; i < road.StopCount; i++)
        {
            float stop = road.StopDistance(i);
            if (Mathf.Abs(stop - d) <= Mathf.Max(0f, snapToStone)) { _goal = stop; return; }
        }
        _goal = Mathf.Clamp(d, 0f, road.Length);
    }

    /// <summary>
    /// Fire onArrive / onLeave from PROXIMITY rather than from a script. When the
    /// walk is on rails the coroutine knows when it has arrived; driven by hand,
    /// arriving is simply being near enough to a word, and leaving is not being.
    /// </summary>
    void NoteWhereWeAre()
    {
        int near = -1;
        float best = Mathf.Max(0.05f, arriveRadius);
        for (int i = 0; i < road.StopCount; i++)
        {
            float d = Mathf.Abs(road.StopDistance(i) - Distance);
            if (d < best) { best = d; near = i; }
        }
        if (near == CurrentIndex) return;

        if (CurrentIndex >= 0) onLeave.Invoke(CurrentIndex);
        CurrentIndex = near;
        CurrentStone = near >= 0 ? road.Stone(near) : null;
        if (near >= 0) onArrive.Invoke(near);
    }

    // ── the free walk, indoors ──────────────────────────────────────────────

    /// <summary>True while the reader is walking the library floor, off the road.</summary>
    public bool Roaming { get; private set; }

    Vector3 _roamGoal;
    bool _hasRoamGoal;
    Bounds[] _solid;

    void EnterRoam()
    {
        Halt();                       // no coroutine may steer them in here
        if (libraryDoor == null)
        {
            var station = FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
            if (station != null) libraryDoor = station.doorway;
        }
        Roaming = true;
        _offRoad = true;
        _hasRoamGoal = false;
        _solid = null;                // measure the furniture on the way in
        Log("free walk: the library floor is theirs.");
    }

    /// <summary>
    /// Step back onto the road at whatever point they walked out at, so leaving by
    /// the door rejoins the walk instead of teleporting them to the head of it.
    /// </summary>
    void LeaveRoam()
    {
        Roaming = false;
        _hasRoamGoal = false;
        if (road == null || road.StopCount == 0) return;

        _offRoad = false;
        Distance = Mathf.Clamp(NearestRoadOutside(transform.position), 0f, road.Length);
        _goal = Distance;             // stand still until asked to move
        Log("back on the road at " + Distance.ToString("0.0") + " m.");
    }

    /// <summary>
    /// The nearest point on the road that is NOT in the library.
    ///
    /// The road runs a loop through the hall (the authored tour, from before the
    /// floor could be walked freely), so the nearest road point to a reader stepping
    /// out of the door is usually a waypoint back INSIDE it. Rejoining there puts
    /// them straight back in the room, which puts them straight back into the free
    /// walk — and the door becomes a wall you can see through. Rejoining outside is
    /// the only reading of "they have left" that lets them leave.
    /// </summary>
    float NearestRoadOutside(Vector3 p)
    {
        float best = road.NearestDistance(p), bestD = float.MaxValue;
        bool found = false;

        for (float d = 0f; d <= road.Length; d += 0.25f)
        {
            Vector3 q = road.PositionAt(d);
            if (LibraryFloor.Contains(q, -0.2f)) continue;      // still in the room
            float sq = (q - p).sqrMagnitude;
            if (sq < bestD) { bestD = sq; best = d; found = true; }
        }
        if (!found)
            Debug.LogWarning("[Walk] Every point on the road is inside the library, " +
                             "so there is nowhere outside to rejoin it.");
        return best;
    }

    /// <summary>
    /// Walk a room rather than a line.
    ///
    /// The inputs are the SAME two the compass and the keys already set — there is
    /// no second movement system here, only a second reading of the two numbers.
    /// Outdoors, forward means "further along the river" and turning is a thing you
    /// do standing still, because steering could only ever fight the path. Indoors
    /// there is no path to fight: forward is the way the reader faces, and turning
    /// steers. That is the whole of it.
    ///
    /// A tap on the floor still works, and still means "go there" — it is the one
    /// control a phone is actually good at.
    /// </summary>
    void Roam()
    {
        if (followPointer && PointerPressed(out Vector2 screen)) AimAtFloor(screen);

        float turn = Mathf.Clamp(RoamTurnAxis() + driveTurn, -1f, 1f);
        if (Mathf.Abs(turn) > 0.01f)
        {
            transform.Rotate(0f, turn * roamTurnSpeed * Time.deltaTime, 0f);
            _hasRoamGoal = false;     // taking the wheel cancels where they were sent
        }

        Vector3 here = transform.position;
        Vector3 step = Vector3.zero;

        float forward = Mathf.Clamp(RoamForwardAxis() + driveForward, -1f, 1f);
        if (Mathf.Abs(forward) > 0.01f)
        {
            step = Flat(transform.forward) * (forward * roamSpeed * Time.deltaTime);
            _hasRoamGoal = false;
        }
        else if (_hasRoamGoal)
        {
            Vector3 to = _roamGoal - here; to.y = 0f;
            float left = to.magnitude;
            if (left < 0.12f) _hasRoamGoal = false;
            else
            {
                step = to / left * Mathf.Min(roamSpeed * Time.deltaTime, left);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, Quaternion.LookRotation(to / left, Vector3.up),
                    turnSpeed * Time.deltaTime);
            }
        }

        if (step.sqrMagnitude < 1e-8f) return;

        Vector3 want = here + step;

        // The walls, with one gap in them. Clamping every axis would seal the reader
        // in the library for good — they have to be able to walk back out to the
        // river, and the way out is the way they came in.
        want = InTheDoorway(want) ? Doorframe(want) : LibraryFloor.KeepOn(want, roamEdge);
        want = OffTheFurniture(want, here);
        want.y = here.y;                              // the floor is flat

        float moved = Vector3.Distance(want, here);
        transform.position = want;
        if (actor != null && moved > 0f) actor.Step(moved);
    }

    /// <summary>Send the reader to a tapped point on the library floor.</summary>
    void AimAtFloor(Vector2 screen)
    {
        var cam = Camera.main;
        if (cam == null) return;

        var floor = new Plane(Vector3.up, transform.position);
        if (!floor.Raycast(cam.ScreenPointToRay(screen), out float enter)) return;

        Vector3 p = cam.ScreenPointToRay(screen).GetPoint(enter);
        _roamGoal = LibraryFloor.KeepOn(p, roamEdge);
        _hasRoamGoal = true;
    }

    /// <summary>
    /// Is this point in the doorway? Which way the door faces is not assumed — it
    /// is the axis from the middle of the floor out to the door itself, so the
    /// building can be turned round in Blender and this still knows the way out.
    /// </summary>
    bool InTheDoorway(Vector3 p)
    {
        if (libraryDoor == null || !LibraryFloor.Known) return false;

        Vector3 door = libraryDoor.position;
        Vector3 outward = door - LibraryFloor.Plan.center; outward.y = 0f;
        bool alongX = Mathf.Abs(outward.x) > Mathf.Abs(outward.z);

        float lateral = alongX ? Mathf.Abs(p.z - door.z) : Mathf.Abs(p.x - door.x);
        if (lateral > Mathf.Max(0.3f, roamDoorWidth * 0.5f)) return false;

        // and only on the door's own side of the room
        float along = alongX ? (p.x - LibraryFloor.Plan.center.x) * Mathf.Sign(outward.x)
                             : (p.z - LibraryFloor.Plan.center.z) * Mathf.Sign(outward.z);
        return along > 0f;
    }

    /// <summary>In the doorway only the jambs hold them; the way out is open.</summary>
    Vector3 Doorframe(Vector3 p)
    {
        Vector3 door = libraryDoor.position;
        Vector3 outward = door - LibraryFloor.Plan.center; outward.y = 0f;
        float half = Mathf.Max(0.3f, roamDoorWidth * 0.5f);

        if (Mathf.Abs(outward.x) > Mathf.Abs(outward.z))
            p.z = Mathf.Clamp(p.z, door.z - half, door.z + half);
        else
            p.x = Mathf.Clamp(p.x, door.x - half, door.x + half);
        return p;
    }

    /// <summary>
    /// Push the reader out of the furniture.
    ///
    /// Boxes rather than circles, because a reading desk is a metre wide and half a
    /// metre deep and a circle round it reserves the difference as floor nobody may
    /// stand on. Pushed out along the SHALLOWEST axis, which is the direction they
    /// came from for any step small enough to be one frame's walk.
    /// </summary>
    Vector3 OffTheFurniture(Vector3 p, Vector3 from)
    {
        if (roamObstacles == null || roamObstacles.Length == 0) return p;
        if (_solid == null) MeasureFurniture();

        float pad = Mathf.Max(0f, roamClearance);
        foreach (var b in _solid)
        {
            float dx = b.extents.x + pad - Mathf.Abs(p.x - b.center.x);
            float dz = b.extents.z + pad - Mathf.Abs(p.z - b.center.z);
            if (dx <= 0f || dz <= 0f) continue;               // clear of it

            if (dx < dz) p.x += Mathf.Sign(p.x - b.center.x) * dx;
            else p.z += Mathf.Sign(p.z - b.center.z) * dz;
        }
        // A push can only ever have made things worse if it put them off the floor.
        return LibraryFloor.Contains(p, 0f) || InTheDoorway(p) ? p : from;
    }

    /// <summary>Measure the solid things in the hall, once.</summary>
    void MeasureFurniture()
    {
        var found = new System.Collections.Generic.List<Bounds>();
        foreach (var r in FindObjectsByType<MeshRenderer>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (r == null) continue;
            bool named = false;
            foreach (var n in roamObstacles)
                if (!string.IsNullOrEmpty(n) && r.name.StartsWith(n)) { named = true; break; }
            if (!named) continue;

            // Only what is standing in THIS room. The kit is instanced all over the
            // island and a column on the terrace is not in the reader's way.
            var b = r.bounds;
            if (!LibraryFloor.Contains(new Vector3(b.center.x, LibraryFloor.FloorY, b.center.z),
                                       -1.0f)) continue;
            found.Add(b);
        }
        _solid = found.ToArray();
        Log($"free walk: {_solid.Length} solid thing(s) in the hall.");
    }

    /// <summary>Forward / back for the free walk: W / S / up / down.</summary>
    static float RoamForwardAxis()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return 0f;
        float a = 0f;
        if (k.upArrowKey.isPressed || k.wKey.isPressed) a += 1f;
        if (k.downArrowKey.isPressed || k.sKey.isPressed) a -= 1f;
        return a;
#elif ENABLE_LEGACY_INPUT_MANAGER
        float a = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) a += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) a -= 1f;
        return a;
#else
        return 0f;
#endif
    }

    /// <summary>Turn for the free walk: A / D / left / right. Outdoors these are
    /// forward and back along the road, which is why this is its own axis.</summary>
    static float RoamTurnAxis()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return 0f;
        float a = 0f;
        if (k.rightArrowKey.isPressed || k.dKey.isPressed) a += 1f;
        if (k.leftArrowKey.isPressed || k.aKey.isPressed) a -= 1f;
        return a;
#elif ENABLE_LEGACY_INPUT_MANAGER
        float a = 0f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) a += 1f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) a -= 1f;
        return a;
#else
        return 0f;
#endif
    }

    void Log(string msg) { if (logRoam) Debug.Log("[Walk] " + msg); }

    /// <summary>
    /// A click or tap THIS FRAME — pressed, not held, and not on the compass.
    ///
    /// The pad sits over the world, so without the UI test every press of an arrow
    /// is also a tap on the floor behind it: the reader is sent to wherever the
    /// button happens to be covering and then walks off there the moment the finger
    /// lifts. Whatever the UI has taken, the world has not.
    /// </summary>
    static bool PointerPressed(out Vector2 pos)
    {
        pos = default;
        var es = EventSystem.current;
        if (es != null && es.IsPointerOverGameObject()) return false;
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        { pos = Touchscreen.current.primaryTouch.position.ReadValue(); return true; }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        { pos = Mouse.current.position.ReadValue(); return true; }
        return false;
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        { pos = Input.GetTouch(0).position; return true; }
        if (Input.GetMouseButtonDown(0)) { pos = Input.mousePosition; return true; }
        return false;
#else
        return false;
#endif
    }

    /// <summary>Forward / back on the keyboard: +1 onward, -1 back, 0 idle.</summary>
    static float KeyAxis()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return 0f;
        float a = 0f;
        if (k.upArrowKey.isPressed || k.wKey.isPressed || k.rightArrowKey.isPressed) a += 1f;
        if (k.downArrowKey.isPressed || k.sKey.isPressed || k.leftArrowKey.isPressed) a -= 1f;
        return a;
#elif ENABLE_LEGACY_INPUT_MANAGER
        float a = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.RightArrow)) a += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.LeftArrow)) a -= 1f;
        return a;
#else
        return 0f;
#endif
    }

    // ── the walk ────────────────────────────────────────────────────────────

    IEnumerator Walk()
    {
        for (int i = 0; i < road.StopCount; i++)
        {
            yield return MoveTo(road.StopDistance(i));

            CurrentIndex = i;
            CurrentStone = road.Stone(i);
            yield return FaceStone();
            onArrive.Invoke(i);

            yield return Dwell();

            onLeave.Invoke(i);
            CurrentStone = null;
            CurrentIndex = -1;
        }

        // past the last word: the dock, the climb, the mat
        yield return MoveTo(road.Length);
        yield return SitDown();
        Seated = true;
        onSeated.Invoke();
    }

    IEnumerator MoveTo(float target)
    {
        while (Distance < target - 0.01f)
        {
            float step = Mathf.Min(walkSpeed * Time.deltaTime, target - Distance);
            Distance += step;
            Place(Distance, snapFacing: false);
            if (actor != null) actor.Step(step);
            yield return null;
        }
        Distance = target;
        Place(Distance, snapFacing: false);
    }

    IEnumerator Dwell()
    {
        _skipDwell = false;
        var stone = CurrentStone != null ? CurrentStone.GetComponent<WordStone>() : null;
        if (advanceOnTap && stone != null)
            stone.onWordClicked.AddListener(OnStoneTapped);

        float t = 0f;
        while (!_skipDwell)
        {
            bool held = waitForKeyStones && stone != null && stone.isKeyword && !stone.solved;
            if (!held && t >= dwellSeconds) break;
            t += Time.deltaTime;
            yield return null;
        }

        if (advanceOnTap && stone != null)
            stone.onWordClicked.RemoveListener(OnStoneTapped);
    }

    void OnStoneTapped(string word, bool isKey) { _skipDwell = true; }

    IEnumerator FaceStone()
    {
        if (CurrentStone == null) yield break;
        Vector3 to = CurrentStone.position - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f) yield break;

        var want = Quaternion.LookRotation(to.normalized, Vector3.up);
        while (Quaternion.Angle(transform.rotation, want) > 1.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, want, turnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = want;
    }

    IEnumerator SitDown()
    {
        // turn to face the way the mat faces, then fold down onto it
        var seat = road.seat;
        var want = seat != null
            ? Quaternion.LookRotation(Flat(seat.forward), Vector3.up)
            : transform.rotation;

        while (Quaternion.Angle(transform.rotation, want) > 1.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, want, turnSpeed * 0.6f * Time.deltaTime);
            yield return null;
        }

        if (actor != null) actor.SetSeated(true);
        yield return new WaitForSeconds(0.8f);
    }

    void Place(float d, bool snapFacing)
    {
        transform.position = road.PositionAt(d);
        Vector3 fwd = Flat(road.TangentAt(d));
        if (fwd.sqrMagnitude < 0.0001f) return;
        var want = Quaternion.LookRotation(fwd, Vector3.up);
        transform.rotation = snapFacing
            ? want
            : Quaternion.RotateTowards(transform.rotation, want,
                                       turnSpeed * Time.deltaTime);
    }

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v.sqrMagnitude > 1e-6f ? v.normalized : v; }
}
