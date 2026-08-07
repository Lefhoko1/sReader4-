// ===========================================================================
//  BookWalkFlow — the conductor: book → sentence → river → walk → book
// ===========================================================================
//  This is the loop the game is actually about:
//
//      the reader walks up to the book at the side of the world
//        → the camera comes round onto the open page
//        → you click a sentence
//        → its words burst off the page and land as stones on the river
//        → the reader leaves the lectern and walks the sentence, word by word
//        → they climb to the library and sit on the mat
//        → they walk back to the book, it turns the page, and round again
//
//  WHY A CONDUCTOR. Every piece of this already existed and none of them could
//  be in the same scene: StoryBook rebuilds the river whenever a sentence is
//  clicked, and PathWalker walks whatever stones it finds — so the setup menu
//  used to switch the book OFF before building the walk. Nothing was wrong with
//  either; they simply both wanted to decide when the river changes. This file
//  is that decision, in one place: the river is only ever rebuilt while the
//  reader is standing at the lectern, never under their feet.
//
//  It drives them through seams that already existed and adds no rules of its
//  own — StoryBook.onSentenceSent, PathWalker.WalkTo / Begin / Seated,
//  RiversideRoad.Rebuild, WalkCamera.focus.
//
//  MVVM note: a presenter. It sequences existing presenters and raises an event
//  when the paragraph is finished. It scores nothing, saves nothing, and asks no
//  service anything — hang the assignment logic off onParagraphFinished.
// ===========================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BookWalkFlow : MonoBehaviour
{
    [Header("Wiring (auto-found if left empty)")]
    public StoryBook book;
    public BookStation station;
    public PathWalker walker;
    public RiversideRoad road;
    public WordPathBuilder path;
    public WalkCamera cam;

    [Header("Opening")]
    [Tooltip("The reader starts this far short of the lectern, so the first thing " +
             "you see is them walking up to the book rather than already standing " +
             "at it. 0 = start at the book.")]
    public float startBackOff = 5f;

    [Header("Arriving at the library")]
    [Tooltip("How long the camera holds on the HALL before it settles onto the " +
             "page. Without this beat the player is dropped straight into a " +
             "close-up of a book and never sees the room they walked into — the " +
             "shelves, the columns, the light. 0 disables the reveal.")]
    public float revealSeconds = 2.4f;
    [Range(0.08f, 0.6f)]
    [Tooltip("How much of the frame the hall fills during the reveal. Small = wide " +
             "and roomy. The camera eases from this to the book's own framing, so " +
             "the move reads as one shot rather than a cut.")]
    public float revealFill = 0.16f;
    [Tooltip("Reveal on the FIRST entry only. Off = the room is re-established " +
             "every time the reader comes back for the next sentence, which gets " +
             "tiresome fast.")]
    public bool revealOnceOnly = true;

    [Header("Inside the library, the walk is the player's")]
    [Tooltip("Do not march the reader to the lectern. Once they are through the " +
             "door the library is theirs to walk — up the aisles, round the desk, " +
             "turning on the spot to see what is on the shelves — and the book only " +
             "takes the frame when they reach it. Untick to go back to the reader " +
             "being carried straight to the page.")]
    public bool freeWalkInTheLibrary = true;
    [Tooltip("How near the reading spot counts as arriving at the book, in metres.")]
    public float readRadius = 1.3f;

    [Header("Beats (seconds)")]
    [Tooltip("Pause after arriving at the book, before it becomes clickable — long " +
             "enough for the camera to settle onto the page.")]
    public float beatAtBook = 0.7f;
    [Tooltip("Pause after the last word lands on the river, before the reader sets off.")]
    public float beatAfterLanding = 0.9f;
    [Tooltip("Pause on the mat at the end of the sentence, before walking back.")]
    public float beatOnTheMat = 1.2f;

    [Header("Behaviour")]
    [Tooltip("Turn the page and go again until the paragraph runs out. Off = stop " +
             "after one sentence.")]
    public bool turnPages = true;
    [Tooltip("Walk from the lectern down to the head of the river on foot. The river " +
             "is read from its far end inwards, so if the book stands up at the " +
             "library that walk is the whole river backwards — untick this and the " +
             "reader is placed at the head of the river instead, with the camera " +
             "travelling there. Tick it if you move the book near the water.")]
    public bool walkOutToTheRiver = true;
    [Tooltip("Print each step of the loop to the Console.")]
    public bool logSteps = true;

    [Tooltip("Fired when the last sentence of the paragraph has been walked. This " +
             "is the seam for scoring / submission — put a ViewModel call here, " +
             "never in this file.")]
    public UnityEvent onParagraphFinished = new UnityEvent();

    bool _sentenceSent;
    bool _revealed;
    // The book's authored framing, captured before the reveal widens it. Reading
    // it back off the camera afterwards would just return the wide value.
    float _bookFill = -1f;

    // ── setup ───────────────────────────────────────────────────────────────

    void Awake()
    {
        Resolve();

        // Nothing else may start a walk: this file owns when the reader moves.
        if (walker != null) walker.autoPlay = false;

        var demo = FindAnyObjectByType<RiversideWalkDemo>();
        if (demo != null && demo.enabled)
        {
            demo.enabled = false;
            Log("switched off RiversideWalkDemo — the book conducts the walk now.");
        }
    }

    void Resolve()
    {
        if (book == null) book = FindAnyObjectByType<StoryBook>(FindObjectsInactive.Include);
        if (station == null) station = FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (walker == null) walker = FindAnyObjectByType<PathWalker>();
        if (road == null) road = FindAnyObjectByType<RiversideRoad>();
        if (path == null) path = FindAnyObjectByType<WordPathBuilder>();
        if (cam == null) cam = FindAnyObjectByType<WalkCamera>();
    }

    IEnumerator Start()
    {
        Resolve();
        if (!Ready()) yield break;

        // Park the reader and compose the camera NOW, before anything is rendered,
        // so frame one is the opening shot and not wherever the scene was last
        // saved. With a doorway that opening shot is the ROOM — starting on a
        // close-up of the book and then cutting out to the hall would throw the
        // reveal away before it happens.
        book.interactive = false;
        bool reveals = station.doorway != null && revealSeconds > 0f;
        if (cam != null)
        {
            _bookFill = cam.focusFill;
            cam.focus = reveals ? station.HallFocus : station.FocusTarget;
            if (reveals) cam.focusFill = revealFill;
        }
        ParkShortOfTheBook();

        // one frame, so StoryBook.Start has laid its first sentence out (and, with
        // Reveal Words On Landing on, raised that sentence's blank stones)
        yield return null;
        if (cam != null) cam.Compose();

        yield return Run();
    }

    bool Ready()
    {
        string missing =
            (book == null ? " StoryBook" : "") +
            (station == null ? " BookStation" : "") +
            (walker == null ? " PathWalker" : "") +
            (road == null ? " RiversideRoad" : "");
        if (missing.Length == 0) return true;

        Debug.LogError($"[BookWalk] Missing:{missing}. Run Tools ▸ Great Library ▸ " +
                       "Island ▸ 9. Build Book Reading Loop.");
        return false;
    }

    // ── the loop ────────────────────────────────────────────────────────────

    IEnumerator Run()
    {
        yield return GoToBook();

        while (true)
        {
            yield return WaitForSentence();
            yield return WalkTheSentence();
            yield return GoToBook();

            if (!turnPages || !book.HasNextPage) break;
            yield return TurnPage();
        }

        Log("paragraph finished.");
        onParagraphFinished.Invoke();
    }

    /// <summary>
    /// Get to the lectern. Inside the library that is the PLAYER'S walk, not ours.
    ///
    /// Marching the reader to the book is the efficient version of this and it costs
    /// the room: the player is carried past the shelves, the globe and the lit
    /// windows on rails, arrives at a page, and never once chooses to look at
    /// anything. So indoors this waits instead. The library is theirs to walk; the
    /// book opens when they get to it, because getting to it is the point.
    /// </summary>
    IEnumerator GoToBook()
    {
        book.interactive = false;
        yield return Reveal();

        Vector3 spot = station.StandPosition;

        if (FreeWalk)
        {
            walker.Stand();                     // never wait for someone who is sitting
            yield return StepInside();
            Log("the library is yours — walk to the book when you are ready.");

            // The camera is deliberately NOT put on the book yet. Framing the page
            // from across the room while the player is trying to look at the shelves
            // is the same railroading with the reader left free to walk under it.
            if (cam != null) cam.focus = null;

            float r = Mathf.Max(0.4f, readRadius);
            yield return new WaitUntil(
                () => Flat(walker.transform.position - spot).sqrMagnitude <= r * r);

            if (cam != null) cam.focus = station.FocusTarget;
            yield return walker.Turn(station.LookAt - walker.transform.position);
        }
        else
        {
            if (cam != null) cam.focus = station.FocusTarget;
            Log($"walking to the book ({Vector3.Distance(walker.transform.position, spot):0.0} m).");
            yield return walker.WalkTo(spot, walker.transitSpeed, station.LookAt - spot);
        }

        yield return new WaitForSeconds(Mathf.Max(0f, beatAtBook));
        book.RefreshTextFacing();
    }

    /// <summary>
    /// Is the reader's walk theirs? Only where there is a library to walk in — out
    /// on the river the road is the game and handing over would strand them.
    /// </summary>
    bool FreeWalk => freeWalkInTheLibrary && walker != null && walker.roamIndoors &&
                     station != null && station.doorway != null;

    /// <summary>
    /// Carry the reader over the threshold, and no further.
    ///
    /// The reveal leaves them standing ON the doorway socket, which is out on the
    /// porch between the columns — outside the floor, so the free walk has nothing
    /// to hand over. One stride in puts them on the library's own floor with the
    /// room around them and the walk theirs from that step on. It is the shortest
    /// scripted move in the game and it exists so that the longest one can stop.
    /// </summary>
    IEnumerator StepInside()
    {
        if (!LibraryFloor.Known) yield break;
        if (LibraryFloor.Contains(walker.transform.position, 0.3f)) yield break;

        Vector3 door = station.doorway.position;
        Vector3 inward = Flat(station.LookAt - door);
        if (inward.sqrMagnitude < 1e-4f) yield break;
        inward.Normalize();

        for (float t = 0.2f; t <= 8f; t += 0.2f)
        {
            Vector3 p = door + inward * t;
            p.y = LibraryFloor.FloorY;
            if (!LibraryFloor.Contains(p, 0.4f)) continue;

            Log($"through the door ({t:0.0} m).");
            yield return walker.WalkTo(p, walker.transitSpeed, inward);
            yield break;
        }
        Debug.LogWarning("[BookWalk] Could not find a way in from the doorway — is " +
                         "SOCKET_Entrance still outside the library's own floor?");
    }

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }

    /// <summary>
    /// Come in through the door under a WIDE shot of the hall, and hold it a beat
    /// before the book takes the frame.
    ///
    /// Without this the player is put straight into a close-up of a page. They
    /// walked past shelves, columns and lit windows and never saw any of it — all
    /// that work is on screen for no frames at all. The reveal costs a couple of
    /// seconds once and is the only thing that makes the room register as a place
    /// rather than a backdrop behind a book.
    /// </summary>
    IEnumerator Reveal()
    {
        if (cam == null || station.doorway == null || revealSeconds <= 0f) yield break;
        if (revealOnceOnly && _revealed) yield break;
        _revealed = true;

        if (_bookFill < 0f) _bookFill = cam.focusFill;
        cam.focus = station.HallFocus;
        cam.focusFill = revealFill;                 // wide: the room, not the page

        Vector3 door = station.doorway.position;
        Vector3 inward = station.LookAt - door;
        Log("entering the library — holding on the hall.");
        yield return walker.WalkTo(door, walker.transitSpeed, inward);
        yield return new WaitForSeconds(revealSeconds);

        // Hand the frame back. The camera's own damping carries it from the hall
        // to the page, so this reads as one move instead of a cut.
        cam.focusFill = _bookFill;
    }

    /// <summary>Wait for the player to click a sentence and for its words to land.</summary>
    IEnumerator WaitForSentence()
    {
        _sentenceSent = false;
        book.onSentenceSent.AddListener(OnSentenceSent);
        book.interactive = true;
        Log("at the book — click a sentence.");

        yield return new WaitUntil(() => _sentenceSent);

        book.interactive = false;
        book.onSentenceSent.RemoveListener(OnSentenceSent);
        yield return new WaitForSeconds(Mathf.Max(0f, beatAfterLanding));
    }

    void OnSentenceSent(string text, int index)
    {
        _sentenceSent = true;
        Log($"sentence sent to the river: \"{text}\"");
    }

    /// <summary>Lay the road along the sentence just built, then walk it to the mat.</summary>
    IEnumerator WalkTheSentence()
    {
        if (cam != null) cam.focus = null;      // release the book; back to the walk shots

        // Safe to rebuild here and ONLY here: the reader is standing at the lectern,
        // not on a stone that is about to move.
        road.Rebuild();
        if (road.StopCount == 0)
        {
            Debug.LogWarning("[BookWalk] The sentence produced no stones to walk. " +
                             "Check the book's Rebuild Stones Per Sentence and that " +
                             "the WordPathBuilder has its stone prefabs.");
            yield break;
        }

        float startD = Mathf.Max(0f, road.StopDistance(0) - Mathf.Max(0f, walker.approach));
        Vector3 head = road.PositionAt(startD);
        Log($"road laid: {road.StopCount} stones, {road.Length:0.0} m.");

        if (walkOutToTheRiver)
            yield return walker.WalkTo(head, walker.transitSpeed, road.TangentAt(startD));

        // Begin() puts the reader on the head of the road either way — after the
        // walk above that is where they already are, so it is invisible; without it
        // it is the cut, and the camera catches up on its own damping.
        walker.Begin();
        yield return new WaitUntil(() => walker.Seated);
        yield return new WaitForSeconds(Mathf.Max(0f, beatOnTheMat));
    }

    /// <summary>
    /// Turn to the next sentence. Only ever called with the reader back at the
    /// lectern, because showing a page re-raises the river for the new sentence.
    /// </summary>
    IEnumerator TurnPage()
    {
        book.TurnPage(+1);
        yield return null;                              // let the turn start
        yield return new WaitWhile(() => book.Busy);
        book.RefreshTextFacing();
        Log($"turned to sentence {book.Page + 1}/{book.SentenceCount}.");
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    void ParkShortOfTheBook()
    {
        // With a doorway the reader starts OUTSIDE it, on the steps, facing in —
        // so the opening frame is the library seen from its own threshold rather
        // than a reader already standing at the desk with the room behind them.
        if (station.doorway != null)
        {
            Vector3 door = station.doorway.position;
            Vector3 inward = station.LookAt - door;
            inward.y = 0f;
            inward = inward.sqrMagnitude > 1e-4f ? inward.normalized : Vector3.forward;

            walker.transform.position = door - inward * Mathf.Max(0.5f, startBackOff * 0.5f);
            walker.transform.rotation = Quaternion.LookRotation(inward, Vector3.up);
            return;
        }

        Vector3 spot = station.StandPosition;
        Vector3 back = spot - station.LookAt;
        back.y = 0f;
        back = back.sqrMagnitude > 1e-4f ? back.normalized : Vector3.back;

        walker.transform.position = spot + back * Mathf.Max(0f, startBackOff);
        walker.transform.rotation = Quaternion.LookRotation(-back, Vector3.up);
    }

    void Log(string msg) { if (logSteps) Debug.Log("[BookWalk] " + msg); }
}
