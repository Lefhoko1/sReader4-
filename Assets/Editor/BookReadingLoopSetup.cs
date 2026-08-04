// ===========================================================================
//  BookReadingLoopSetup — one click: the book joins the walk (Editor)
// ===========================================================================
//  Tools > Great Library > Island >
//     9.  Build Book Reading Loop     — stands the book on a plinth at the side
//                                       of the world and wires the loop
//     9b. Re-place Book Nook          — put the nook back where this tool would
//                                       put it (after you have dragged it about)
//     Remove Book Reading Loop
//
//  RUN STEP 6 FIRST. This builds ON the riverside walk: it needs the road, the
//  reader and the walking camera that "6. Build Riverside Walk" makes. Step 6
//  deliberately switches the book OFF (the two used to fight over the river);
//  this switches it back on and puts the conductor between them, so re-running
//  step 6 means re-running this one afterwards.
//
//  WHAT IT MAKES
//      RiversideWalk/
//        BookNook          <- BookStation, on the ground beside the reading mat
//          Pedestal        <- a plinth (no collider: it must not eat page taps)
//          StoryBook       <- the book, tilted like a lectern, facing the walk
//            PageFocus     <- what the camera frames while the reader reads
//        BookFlow          <- BookWalkFlow, the conductor
//
//  MOVING THE BOOK. The nook is a plain transform — drag it anywhere and the
//  reading spot, the reader's approach and the camera shot all follow it, because
//  BookStation derives them from the book instead of storing them.
// ===========================================================================
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BookReadingLoopSetup
{
    const string WALK_ROOT = "RiversideWalk";
    const string NOOK = "BookNook";
    const string FLOW = "BookFlow";
    const string FBX = "Assets/Art/Models_Book/SM_StoryBook.fbx";

    // the nook, in metres
    const float SIDE_OFFSET = 2.4f;      // how far to the side of the mat it stands
    const float ALONG_OFFSET = 0.8f;     // and how far back from it
    const float PLINTH_HEIGHT = 1.02f;
    const float PLINTH_RADIUS = 0.38f;
    const float BOOK_WIDTH = 0.80f;      // the book is scaled to this across the spread
    const float BOOK_TILT = 30f;         // lectern tilt, degrees
    // The reader stands right beside the lens in the book shot, so an adult-scaled
    // stand-in eats a third of a phone screen. They are a child in this game anyway.
    const float READER_HEIGHT = 1.45f;

    // ======================================================================
    [MenuItem("Tools/Great Library/Island/9. Build Book Reading Loop")]
    public static void Build()
    {
        var road = Object.FindAnyObjectByType<RiversideRoad>();
        var walker = Object.FindAnyObjectByType<PathWalker>();
        var path = Object.FindAnyObjectByType<WordPathBuilder>();
        if (road == null || walker == null || path == null)
        {
            Debug.LogError("[BookWalk] No riverside walk in this scene. Run " +
                           "Tools ▸ Great Library ▸ Island ▸ 6. Build Riverside Walk " +
                           "first — this builds on the road, the reader and the " +
                           "walking camera that step makes.");
            return;
        }

        var camGO = GameObject.Find("Main Camera");
        var walkCam = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (walkCam == null)
        {
            Debug.LogError("[BookWalk] No WalkCamera on 'Main Camera'. Run step 6.");
            return;
        }

        var root = GameObject.Find(WALK_ROOT);
        if (root == null)
        {
            Debug.LogError($"[BookWalk] No '{WALK_ROOT}' object. Run step 6.");
            return;
        }

        // ---------- the river stays in world space -----------------------
        // The book rebuilds the river every time a sentence is clicked, using the
        // builder's CURRENT settings. If the fixed-camera composition crept back
        // on, every new sentence would be laid out for a camera that no longer
        // exists and the reader would walk past sliding stones.
        Undo.RecordObject(path, "Build Book Reading Loop");
        path.perspectiveCompensation = 0f;
        path.frameOnScreen = false;
        path.stoneScreenWidth = 0f;
        path.minFitFactor = 1f;

        // ---------- the book ----------------------------------------------
        var book = FindBook();
        bool newBook = book == null;
        if (newBook)
        {
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(FBX);
            if (model == null)
            {
                Debug.LogError($"[BookWalk] {FBX} not found. Run Tools ▸ Great " +
                               "Library ▸ Book ▸ 1. Setup Book Materials & Import.");
                return;
            }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(model);
            go.name = "StoryBook";
            Undo.RegisterCreatedObjectUndo(go, "Build Book Reading Loop");
            book = Undo.AddComponent<StoryBook>(go);
        }
        else if (!book.gameObject.activeSelf)
        {
            // step 6 stands the book down; the conductor is what lets it back up
            Undo.RecordObject(book.gameObject, "Build Book Reading Loop");
            book.gameObject.SetActive(true);
        }

        // ---------- the nook ----------------------------------------------
        var nook = Ensure(NOOK, root.transform);
        bool newNook = nook.GetComponent<BookStation>() == null;
        var station = GetOrAdd<BookStation>(nook);

        Undo.RecordObject(book.transform, "Build Book Reading Loop");
        book.transform.SetParent(nook.transform, true);

        if (newNook || newBook) PlaceNook(nook, book, road);
        BuildPlinth(nook);
        var focus = EnsureFocus(book);

        station.book = book.transform;
        station.focusTarget = focus;
        station.standDistance = 0.95f;
        station.standSide = 0.35f;
        EditorUtility.SetDirty(station);

        // ---------- the book's settings -----------------------------------
        Undo.RecordObject(book, "Build Book Reading Loop");
        book.oneSentencePerSpread = true;        // one sentence gets the whole spread
        book.rebuildStonesPerSentence = true;    // one stone per word of THAT sentence
        book.revealWordsOnLanding = true;        // stones rise blank, chips carry the words
        book.keepFacingCamera = true;            // the camera moves now
        book.interactive = false;                // the conductor opens it
        book.stonePath = path.transform;
        EditorUtility.SetDirty(book);

        // ---------- the conductor ------------------------------------------
        var flowGO = Ensure(FLOW, root.transform);
        var flow = GetOrAdd<BookWalkFlow>(flowGO);
        Undo.RecordObject(flow, "Build Book Reading Loop");
        flow.book = book;
        flow.station = station;
        flow.walker = walker;
        flow.road = road;
        flow.path = path;
        flow.cam = walkCam;
        // The river is read from its far end inwards and the book stands up at the
        // library, so walking out to the head of it on foot is the whole river
        // backwards. Move the nook down to the water and tick this back on.
        flow.walkOutToTheRiver = false;
        EditorUtility.SetDirty(flow);

        // the old self-playing demo would start a second walk on top of this one
        var demo = Object.FindAnyObjectByType<RiversideWalkDemo>();
        if (demo != null && demo.gameObject.activeSelf)
        {
            Undo.RecordObject(demo.gameObject, "Build Book Reading Loop");
            demo.gameObject.SetActive(false);
        }
        Undo.RecordObject(walker, "Build Book Reading Loop");
        walker.autoPlay = false;

        // ---------- park the opening frame ---------------------------------
        Undo.RecordObject(walkCam, "Build Book Reading Loop");
        walkCam.focus = focus;
        FrameBookShot(walkCam);
        SizeReader(walkCam);
        ParkReader(walker, station, flow.startBackOff);
        Undo.RecordObject(walkCam.transform, "Build Book Reading Loop");
        walkCam.Compose();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = nook;
        EditorGUIUtility.PingObject(nook);

        var size = Bounds(book.gameObject);
        Debug.Log(
            "[BookWalk] Book reading loop built.\n" +
            $"  • the book stands at {nook.transform.position}, " +
            $"reader reads from {station.StandPosition}\n" +
            $"  • book measures {(size.HasValue ? size.Value.size.ToString("0.00") : "?")} m " +
            $"(scale {book.transform.localScale.x:0.000}) — it should be about " +
            $"{BOOK_WIDTH:0.00} m across. Wildly bigger or smaller than that is why " +
            "you would be staring at a wall of page or hunting for a speck.\n" +
            "  • press Play: the reader walks up to the book, you click a sentence, " +
            "its words fly onto the river, and the reader walks it to the mat — " +
            "then comes back and the page turns.\n" +
            "  • drag 'BookNook' anywhere and the reading spot, the approach and " +
            "the camera shot all follow it (7b puts it back).\n" +
            "  • 'BookFlow ▸ Walk Out To The River' is off: the reader is placed at " +
            "the head of the river rather than walking the whole river backwards to " +
            "reach it. Move the nook down to the water and you can tick it on.\n" +
            "  • re-running step 6 switches the book off again — re-run this after it.");
    }

    [MenuItem("Tools/Great Library/Island/9. Build Book Reading Loop", true)]
    static bool BuildValidate() => !Application.isPlaying;

    // ======================================================================
    [MenuItem("Tools/Great Library/Island/9b. Re-place Book Nook")]
    public static void ReplaceNook()
    {
        var nook = GameObject.Find(NOOK);
        var station = nook != null ? nook.GetComponent<BookStation>() : null;
        var road = Object.FindAnyObjectByType<RiversideRoad>();
        var walker = Object.FindAnyObjectByType<PathWalker>();
        if (station == null || road == null || walker == null)
        {
            Debug.LogError("[BookWalk] Nothing to re-place. Run step 9 first.");
            return;
        }

        var book = station.Book.GetComponent<StoryBook>();
        if (book == null)
        {
            Debug.LogError("[BookWalk] The station has no StoryBook under it. Run step 9.");
            return;
        }
        PlaceNook(nook, book, road);
        BuildPlinth(nook);

        var camGO = GameObject.Find("Main Camera");
        var walkCam = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (walkCam != null)
        {
            Undo.RecordObject(walkCam.transform, "Re-place Book Nook");
            Undo.RecordObject(walkCam, "Re-place Book Nook");
            walkCam.focus = station.FocusTarget;
            FrameBookShot(walkCam);
            SizeReader(walkCam);
            var flow = Object.FindAnyObjectByType<BookWalkFlow>();
            ParkReader(walker, station, flow != null ? flow.startBackOff : 5f);
            walkCam.Compose();
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = nook;
        Debug.Log($"[BookWalk] Nook back at {nook.transform.position}.");
    }

    /// <summary>
    /// Re-park the reader and put the camera back on the opening frame. Public so
    /// anything that moves the book — the dressing pass drops it onto a desk — can
    /// ask for the shot to be re-composed without duplicating the composition.
    /// </summary>
    public static void Recompose()
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        var walker = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
        var camGO = GameObject.Find("Main Camera");
        var walkCam = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (station == null || walker == null || walkCam == null) return;

        var flow = Object.FindAnyObjectByType<BookWalkFlow>(FindObjectsInactive.Include);
        Undo.RecordObject(walkCam, "Recompose Book Shot");
        walkCam.focus = station.FocusTarget;
        FrameBookShot(walkCam);
        SizeReader(walkCam);
        ParkReader(walker, station, flow != null ? flow.startBackOff : 5f);
        Undo.RecordObject(walkCam.transform, "Recompose Book Shot");
        walkCam.Compose();
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Island/Remove Book Reading Loop")]
    public static void Remove()
    {
        foreach (var n in new[] { FLOW, NOOK })
        {
            var go = GameObject.Find(n);
            if (go != null) Undo.DestroyObjectImmediate(go);
        }

        var camGO = GameObject.Find("Main Camera");
        var wc = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (wc != null)
        {
            Undo.RecordObject(wc, "Remove Book Reading Loop");
            wc.focus = null;
        }

        var demo = Object.FindAnyObjectByType<RiversideWalkDemo>(FindObjectsInactive.Include);
        if (demo != null && !demo.gameObject.activeSelf)
        {
            Undo.RecordObject(demo.gameObject, "Remove Book Reading Loop");
            demo.gameObject.SetActive(true);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[BookWalk] Reading loop removed — the self-playing walk demo is " +
                  "back. The river is still laid out in world space.");
    }

    // ── placement ───────────────────────────────────────────────────────────

    /// <summary>
    /// Stand the nook beside the reading mat, off the walk, facing back across it —
    /// so the reader arrives with the book in front of them and the camera has the
    /// pages square on. Everything is measured from things already in the scene.
    /// </summary>
    static void PlaceNook(GameObject nook, StoryBook book, RiversideRoad road)
    {
        // the mat is the end of the walk; failing that, the end of the road
        Vector3 anchor = road.seat != null ? road.seat.position
                                           : road.PositionAt(road.Length);
        Vector3 outward = road.seat != null
            ? Flat(road.seat.forward)                       // the mat looks out to sea
            : Flat(road.TangentAt(road.Length));
        if (outward.sqrMagnitude < 1e-4f) outward = Vector3.back;
        outward.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, outward).normalized;
        Vector3 pos = anchor + side * SIDE_OFFSET + outward * ALONG_OFFSET;
        pos = OnGround(pos, anchor.y);

        Undo.RecordObject(nook.transform, "Place Book Nook");
        nook.transform.SetPositionAndRotation(pos, Quaternion.identity);

        // The book faces back toward the mat: the reader walks in off the road,
        // stops between the two, and reads with the page square to the camera.
        Vector3 awayFromReader = Flat(pos - anchor);
        if (awayFromReader.sqrMagnitude < 1e-4f) awayFromReader = side;
        awayFromReader.Normalize();

        var bt = book.transform;
        Undo.RecordObject(bt, "Place Book Nook");
        ScaleBook(bt);
        // "Up the page" is the book's forward, so the top of the page points away
        // from the reader; the tilt then lifts that far edge into a lectern.
        bt.rotation = Quaternion.LookRotation(awayFromReader, Vector3.up) *
                      Quaternion.Euler(-BOOK_TILT, 0f, 0f);
        bt.position = pos + Vector3.up * PLINTH_HEIGHT;
        // sit it ON the plinth whatever the model's pivot turns out to be
        var b = Bounds(book.gameObject);
        if (b.HasValue)
            bt.position += Vector3.up * (pos.y + PLINTH_HEIGHT - b.Value.min.y);
    }

    /// <summary>
    /// Drop the nook onto whatever surface is under it. The mat's height is only
    /// the right height WHERE THE MAT IS — a couple of metres to the side the
    /// island has usually sloped away, which leaves the plinth either floating or
    /// buried to its rim. Falls back to the mat's height if nothing down there has
    /// a collider to land on.
    /// </summary>
    static Vector3 OnGround(Vector3 p, float fallbackY)
    {
        var from = new Vector3(p.x, fallbackY + 6f, p.z);
        if (Physics.Raycast(from, Vector3.down, out var hit, 16f) &&
            hit.collider.GetComponentInParent<WordStone>() == null)
        {
            Debug.Log($"[BookWalk] nook dropped onto '{hit.collider.name}' at " +
                      $"y = {hit.point.y:0.00} (the mat sits at {fallbackY:0.00}).");
            p.y = hit.point.y;
        }
        else
        {
            Debug.Log($"[BookWalk] nothing with a collider under the nook — left at " +
                      $"the mat's height, y = {fallbackY:0.00}. If the plinth floats " +
                      "or sinks, just drag BookNook down/up in the Scene view.");
            p.y = fallbackY;
        }
        return p;
    }

    /// <summary>
    /// Scale the book so its spread is <see cref="BOOK_WIDTH"/> across. Public
    /// because the dressing pass moves the book onto a desk and wants the same
    /// size applied there, without having to re-place the whole nook to get it.
    /// Idempotent: it measures what is there and solves for the difference.
    /// </summary>
    public static void ScaleBook(Transform book)
    {
        if (book == null) return;
        var b = Bounds(book.gameObject);
        if (!b.HasValue) return;

        float widest = Mathf.Max(b.Value.size.x, b.Value.size.z);
        float current = book.localScale.x;
        if (widest < 1e-4f || current < 1e-4f) return;

        Undo.RecordObject(book, "Scale Book");
        book.localScale = Vector3.one *
            Mathf.Clamp(BOOK_WIDTH / (widest / current), 0.02f, 50f);
    }

    /// <summary>
    /// Size the reader and tell the camera where their eyes are. Kept with the shot
    /// rather than on the actor's defaults because it is a FRAMING decision — it is
    /// how much of a phone screen a body standing next to the lens is allowed.
    /// </summary>
    public static void SizeReader(WalkCamera wc)
    {
        var actor = Object.FindAnyObjectByType<PlaceholderActor>(FindObjectsInactive.Include);
        if (actor != null && !Mathf.Approximately(actor.height, READER_HEIGHT))
        {
            Undo.RecordObject(actor, "Size Reader");
            actor.height = READER_HEIGHT;
            actor.RebuildBody();
            EditorUtility.SetDirty(actor);
        }
        if (wc != null)
        {
            Undo.RecordObject(wc, "Size Reader");
            wc.headHeight = READER_HEIGHT * 0.9f;
            EditorUtility.SetDirty(wc);
        }
    }

    static Bounds? Bounds(GameObject go)
    {
        var rends = go.GetComponentsInChildren<Renderer>(true)
                      .Where(r => !(r is ParticleSystemRenderer)).ToArray();
        if (rends.Length == 0) return null;
        var b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        return b;
    }

    /// <summary>A plinth to stand the book on. No collider — it must not eat taps.</summary>
    static void BuildPlinth(GameObject nook)
    {
        var t = nook.transform.Find("Pedestal");
        GameObject go;
        if (t == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Pedestal";
            go.transform.SetParent(nook.transform, false);
            Undo.RegisterCreatedObjectUndo(go, "Build Book Reading Loop");
        }
        else go = t.gameObject;

        var col = go.GetComponent<Collider>();
        if (col != null) Undo.DestroyObjectImmediate(col);

        Undo.RecordObject(go.transform, "Build Book Reading Loop");
        go.transform.localPosition = new Vector3(0f, PLINTH_HEIGHT * 0.5f, 0f);
        go.transform.localScale = new Vector3(PLINTH_RADIUS * 2f, PLINTH_HEIGHT * 0.5f,
                                              PLINTH_RADIUS * 2f);

        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null && mr.sharedMaterial != null &&
            mr.sharedMaterial.name.StartsWith("Default"))
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var m = new Material(sh) { name = "M_BookPlinth (runtime)" };
            var c = new Color(0.55f, 0.45f, 0.38f);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            if (m.HasProperty("_Color")) m.SetColor("_Color", c);
            mr.sharedMaterial = m;
        }
    }

    /// <summary>The point the camera frames: the middle of the open spread.</summary>
    static Transform EnsureFocus(StoryBook book)
    {
        var t = book.transform.Find("PageFocus");
        if (t == null)
        {
            var go = new GameObject("PageFocus");
            go.transform.SetParent(book.transform, false);
            Undo.RegisterCreatedObjectUndo(go, "Build Book Reading Loop");
            t = go.transform;
        }
        var b = Bounds(book.gameObject);
        Undo.RecordObject(t, "Build Book Reading Loop");
        t.position = b.HasValue ? b.Value.center : book.transform.position;
        return t;
    }

    /// <summary>
    /// The authored book shot, in one place — the counterpart of step 6b's Reframe.
    /// Worth writing here rather than leaving to the component's field defaults:
    /// once a WalkCamera is serialized in the scene, changing a default in code no
    /// longer reaches it, so this is the only way a re-run actually re-composes.
    /// </summary>
    static void FrameBookShot(WalkCamera wc)
    {
        wc.focusDistance = 2.0f;      // metres; fixed, so a narrow frame can't
        wc.focusMaxPullback = 2.2f;   //   reverse the lens into the hill
        wc.focusFill = 0.62f;         // the page is the subject — let it own the frame
        // High enough to look DOWN ONTO the page. At a shallow angle the text is
        // foreshortened into a grey smear no matter how close the camera gets, and
        // reading it is the entire point of this scene. The book's own 30° lectern
        // tilt does the other half of the work: 42° up + 30° tilted back leaves the
        // lens within about 18° of square to the page.
        wc.focusPitch = 42f;
        wc.focusYaw = 30f;            // degrees round from the reader's shoulder
        wc.focusMinFov = 26f;
        wc.focusMaxFov = 60f;
        EditorUtility.SetDirty(wc);
    }

    /// <summary>Stand the reader back from the book, facing it, ready to walk in.</summary>
    static void ParkReader(PathWalker walker, BookStation station, float backOff)
    {
        Vector3 spot = station.StandPosition;
        Vector3 back = Flat(spot - station.LookAt);
        back = back.sqrMagnitude > 1e-4f ? back.normalized : Vector3.back;

        Undo.RecordObject(walker.transform, "Build Book Reading Loop");
        walker.transform.position = spot + back * Mathf.Max(0f, backOff);
        walker.transform.rotation = Quaternion.LookRotation(-back, Vector3.up);
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    static StoryBook FindBook() =>
        Object.FindAnyObjectByType<StoryBook>(FindObjectsInactive.Include);

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }

    static GameObject Ensure(string name, Transform parent)
    {
        var t = parent.Find(name);
        if (t != null) return t.gameObject;

        var loose = GameObject.Find(name);
        if (loose != null) { loose.transform.SetParent(parent, true); return loose; }

        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Build Book Reading Loop");
        return go;
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = Undo.AddComponent<T>(go);
        return c;
    }
}
