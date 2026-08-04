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

    [Header("Behaviour")]
    public bool autoPlay = true;
    [Tooltip("Hold at a gold key stone until its challenge is solved, instead of " +
             "dwelling for a fixed beat. This is the real game's rule.")]
    public bool waitForKeyStones = false;
    [Tooltip("Tapping the word the reader is standing at sends them on early.")]
    public bool advanceOnTap = true;

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
    Coroutine _run;

    void Start() { if (autoPlay) Begin(); }

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
        Seated = false;
        if (actor != null) actor.SetSeated(false);
        Distance = Mathf.Max(0f, road.StopDistance(0) - Mathf.Max(0f, approach));
        _goal = Distance;          // stand still until the player says otherwise
        Place(Distance, snapFacing: true);

        // Under the finger there is no script to run: Update() drives the walk, and
        // arriving at a word is something that HAPPENS when you get near one rather
        // than something the coroutine decides for you.
        if (!followPointer) _run = StartCoroutine(Walk());
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
        if (!followPointer || road == null || Seated || road.StopCount == 0) return;

        if (PointerPressed(out Vector2 screen)) AimAt(screen);

        float axis = KeyAxis();
        if (Mathf.Abs(axis) > 0.01f)
            _goal = Mathf.Clamp(_goal + axis * walkSpeed * keyNudgeRate * Time.deltaTime,
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

    /// <summary>A click or tap THIS FRAME — pressed, not held.</summary>
    static bool PointerPressed(out Vector2 pos)
    {
        pos = default;
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
