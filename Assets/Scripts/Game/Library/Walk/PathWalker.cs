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

    /// <summary>Which way the reader is heading — the camera leads with this.</summary>
    public Vector3 Heading =>
        road != null ? road.TangentAt(Distance) : transform.forward;

    bool _skipDwell;
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
        Seated = false;
        if (actor != null) actor.SetSeated(false);
        Distance = Mathf.Max(0f, road.StopDistance(0) - Mathf.Max(0f, approach));
        Place(Distance, snapFacing: true);
        _run = StartCoroutine(Walk());
    }

    /// <summary>Send the reader on now, without waiting out the rest of the dwell.</summary>
    public void Continue() { _skipDwell = true; }

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
