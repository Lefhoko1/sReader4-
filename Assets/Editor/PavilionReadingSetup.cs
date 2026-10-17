// ===========================================================================
//  PavilionReadingSetup — the sReader loop, laid through the Pavilion
// ===========================================================================
//  Tools > Great Library > Pavilion > Build Reading Walk
//
//  The island version of this game is a river: a book at one end, word stones
//  across the water, a reader walking them. The Pavilion has no river — it has
//  a corridor, a ramp and a lounge. So the same loop is laid along the route the
//  player already walks: they come in from outside, the stones lead them through
//  the building, and the book is waiting on the reading desk at the end.
//
//  NOTHING NEW IS INVENTED HERE. The stones, the pooling, the sentence-to-stone
//  flight and the book are the same components the island uses; the only thing
//  that changes is the LINE they are laid on. That line is a WordRiverPath —
//  named for the river, but it is only ever a polyline — sampled along the
//  Pavilion's own Room 3 markers and dropped onto the real floor by probe,
//  because the markers are volume centres floating in mid-air and the ramp means
//  the route climbs.
//
//  What this does:
//    1. finds the reading desk (your selection, or by name)
//    2. stands the book on it, facing the chair
//    3. pulls the chair up to the desk
//    4. samples a route from the corridor to the desk, on the floor
//    5. lays the word stones along it and points the book at them
//
//  Safe to re-run: it reuses what it already made.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PavilionReadingSetup
{
    const string ROOT = "sReaderTest";
    const string PATH_GO = "ReadingPath";
    const string BUILDER_GO = "WordPath";
    const string BOOK_GO = "ReadingBook";

    const string BOOK_FBX = "Assets/Art/Models_Book/SM_StoryBook.fbx";
    const string STONE_DIR = "Assets/Art/Models_WordPath";

    /// <summary>Metres between sampled route points. Roughly one stride.</summary>
    const float STEP = 1.2f;

    /// <summary>How much of the desk top the open book covers.</summary>
    const float BOOK_FILL = 0.75f;

    /// <summary>Clear space kept between the last stone and the desk, in metres.</summary>
    const float DESK_CLEARANCE = 0.6f;

    /// <summary>The Pavilion's own Room 3 markers, from the far end inward.</summary>
    static readonly string[] ROUTE =
    {
        "Volume Room 3 Corridor",
        "ReflectionProbe Room 3 Ramp",
        "Volume Room 3",
        "Volume Room 3 Sitting",
    };

    [MenuItem("Tools/Great Library/Pavilion/Build Reading Walk", true)]
    static bool BuildValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Pavilion/Build Reading Walk")]
    public static void Build()
    {
        var log = new List<string>();

        // ---------- the desk ------------------------------------------------
        Transform desk = Selection.activeTransform ?? FindLike("Desk", "Table");
        if (desk == null)
        {
            Debug.LogError(
                "[Reading] No reading desk found. Select the desk in the Hierarchy " +
                "and run this again — the scene file on disk has no object named " +
                "'Desk' or 'Table', so it cannot be guessed.");
            return;
        }
        Bounds deskBounds = WorldBounds(desk);
        log.Add($"desk: '{desk.name}' at {V(desk.position)}, top at y {deskBounds.max.y:0.00}");

        var root = Ensure(ROOT, null);

        // ---------- the book, standing on it --------------------------------
        var book = SetUpBook(root.transform, desk, deskBounds, log);

        // ---------- the chair, pulled up to it ------------------------------
        SeatTheChair(desk, deskBounds, log);

        // ---------- the route: corridor -> desk, on the floor ---------------
        var points = SampleRoute(desk, deskBounds, log);
        if (points.Count < 2)
        {
            Debug.LogError("[Reading] Could not build a route — none of the Pavilion's " +
                           "Room 3 markers were found (" + string.Join(", ", ROUTE) + "). " +
                           "Is SC_Pavilion the open scene?");
            return;
        }

        var pathGO = Ensure(PATH_GO, root.transform);
        var line = GetOrAdd<WordRiverPath>(pathGO);
        Undo.RecordObject(line, "Build Reading Walk");
        line.points = points.ToArray();
        line.Invalidate();
        EditorUtility.SetDirty(line);
        log.Add($"route: {points.Count} point(s), {line.Length:0.0} m from " +
                $"{V(points[0])} to {V(points[points.Count - 1])}");

        // ---------- the stones ----------------------------------------------
        LayTheStones(root.transform, line, points, book, log);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = root;

        Debug.Log("[Reading] Reading walk built through the Pavilion.\n  • " +
                  string.Join("\n  • ", log) +
                  "\n  → press Play, walk in from outside, and click a sentence in the " +
                  "book: its words fly onto the stones along the way in.");
    }

    // ── the book ────────────────────────────────────────────────────────────

    static StoryBook SetUpBook(Transform parent, Transform desk, Bounds deskBounds,
                               List<string> log)
    {
        var existing = Object.FindAnyObjectByType<StoryBook>(FindObjectsInactive.Include);
        GameObject go;

        if (existing != null) { go = existing.gameObject; }
        else
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BOOK_FBX);
            if (prefab == null)
            {
                log.Add($"NO BOOK: {BOOK_FBX} is missing, so nothing was placed to read.");
                return null;
            }
            go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.name = BOOK_GO;
            go.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(go, "Build Reading Walk");
        }

        // Standing ON the desk, at the near edge, turned to face whoever sits down.
        Undo.RecordObject(go.transform, "Build Reading Walk");
        Vector3 toRoom = Flat(deskBounds.center - RoomCentre(deskBounds));
        if (toRoom.sqrMagnitude < 0.01f) toRoom = Vector3.forward;

        go.transform.rotation = Quaternion.LookRotation(toRoom, Vector3.up);

        // SIZED TO THE DESK, not left at whatever the model was authored at. The
        // book is the thing you read a sentence off — at its own scale it lands on
        // that desk like a matchbox, and the page text with it. Fill most of the
        // desk top, then sit it on the surface at the measured height rather than
        // guessing an offset.
        go.transform.localScale = Vector3.one;
        Bounds raw = WorldBounds(go.transform);
        float want = Mathf.Max(deskBounds.size.x, deskBounds.size.z) * BOOK_FILL;
        float have = Mathf.Max(raw.size.x, raw.size.z);
        if (have > 0.001f)
            go.transform.localScale = Vector3.one * Mathf.Clamp(want / have, 0.2f, 12f);

        Bounds sized = WorldBounds(go.transform);
        float lift = go.transform.position.y - sized.min.y;      // pivot vs its underside
        go.transform.position = new Vector3(deskBounds.center.x,
                                            deskBounds.max.y + lift,
                                            deskBounds.center.z);
        log.Add($"book scaled x{go.transform.localScale.x:0.00} to " +
                $"{Mathf.Max(WorldBounds(go.transform).size.x, WorldBounds(go.transform).size.z):0.00} m " +
                $"across, sitting on the desk top");

        var sb = go.GetComponent<StoryBook>() ?? Undo.AddComponent<StoryBook>(go);

        // The mouse in this scene is the camera — the player controller locks the
        // cursor for look-around, so a click can never reach a sentence. BookReader
        // lets the two take turns: walk up, press E, and the mouse is a mouse again.
        var reader = go.GetComponent<BookReader>() ?? Undo.AddComponent<BookReader>(go);
        reader.book = sb;
        EditorUtility.SetDirty(reader);
        log.Add($"book is readable on approach: walk within {reader.range:0.0} m and " +
                $"press E (Esc or E again to go back to walking)");

        EditorUtility.SetDirty(go);
        log.Add($"book: '{go.name}' on the desk at {V(go.transform.position)}" +
                (existing != null ? " (the one already in the scene)" : " (newly placed)"));
        return sb;
    }

    /// <summary>
    /// Pull the chair up to the desk, facing it. Nothing is created — if there is
    /// no chair in the room this simply says so, because inventing furniture in a
    /// dressed sample scene would look worse than leaving it be.
    /// </summary>
    static void SeatTheChair(Transform desk, Bounds deskBounds, List<string> log)
    {
        var chair = FindLike("Chair", "Seat", "Stool");
        if (chair == null) { log.Add("no chair found in the room — desk left unseated."); return; }

        Bounds cb = WorldBounds(chair);
        Vector3 away = Flat(cb.center - deskBounds.center);
        if (away.sqrMagnitude < 0.01f) away = Vector3.forward;

        float gap = deskBounds.extents.magnitude * 0.5f + cb.extents.magnitude * 0.5f;
        Vector3 seat = deskBounds.center + away * gap;

        Undo.RecordObject(chair, "Build Reading Walk");
        chair.position = new Vector3(seat.x, chair.position.y, seat.z);
        chair.rotation = Quaternion.LookRotation(-away, Vector3.up);   // look at the desk
        if (Floor(chair.position, out float y, chair))
            chair.position += Vector3.up * (y - WorldBounds(chair).min.y);
        EditorUtility.SetDirty(chair);
        log.Add($"chair: '{chair.name}' drawn up to the desk at {V(chair.position)}, facing it");
    }

    // ── the route ───────────────────────────────────────────────────────────

    /// <summary>
    /// Walk the Room 3 markers, sample every STEP metres, and drop each sample onto
    /// the floor. The markers are volume centres — they say WHERE the route goes
    /// but nothing about how high the floor is, and the ramp means that changes.
    /// </summary>
    static List<Vector3> SampleRoute(Transform desk, Bounds deskBounds, List<string> log)
    {
        var spine = new List<Vector3>();
        int missing = 0;
        foreach (var n in ROUTE)
        {
            var t = Find(n);
            if (t == null) { missing++; continue; }
            spine.Add(t.position);
        }
        // Finish IN FRONT of the desk, not at its centre. Ending on the centre puts
        // the last word — often the one that completes the sentence — underneath
        // the desk, where it cannot be read or reached.
        if (spine.Count > 0)
        {
            Vector3 approach = spine[spine.Count - 1];
            Vector3 toDesk = Flat(deskBounds.center - approach);
            if (toDesk.sqrMagnitude < 0.01f) toDesk = Vector3.forward;
            float clear = Mathf.Max(deskBounds.extents.x, deskBounds.extents.z) + DESK_CLEARANCE;
            spine.Add(deskBounds.center - toDesk * clear);
        }
        if (missing > 0) log.Add($"{missing} route marker(s) missing — route is shorter.");

        var outPts = new List<Vector3>();
        int grounded = 0;
        for (int i = 1; i < spine.Count; i++)
        {
            Vector3 a = spine[i - 1], b = spine[i];
            float len = Vector3.Distance(a, b);
            int steps = Mathf.Max(1, Mathf.RoundToInt(len / STEP));
            for (int s = (i == 1 ? 0 : 1); s <= steps; s++)
            {
                Vector3 p = Vector3.Lerp(a, b, s / (float)steps);
                if (Floor(p, out float y)) { p.y = y; grounded++; }
                outPts.Add(p);
            }
        }
        log.Add($"{grounded} of {outPts.Count} route point(s) found the floor by probe");
        return outPts;
    }

    // ── the stones ──────────────────────────────────────────────────────────

    static void LayTheStones(Transform root, WordRiverPath line, List<Vector3> points,
                             StoryBook book, List<string> log)
    {
        var go = Ensure(BUILDER_GO, root);
        var b = GetOrAdd<WordPathBuilder>(go);
        Undo.RecordObject(b, "Build Reading Walk");

        // The builder still wants two markers even when a polyline is driving the
        // shape — it measures the run between them. Give it the route's own ends.
        //
        // PARENTED TO THE ROOT, NOT TO THE BUILDER. ClearStones() runs at the top of
        // every build and destroys every child of the builder that is not a
        // StoneSlot — so markers kept there are deleted by the very call that then
        // checks whether they exist, and it reports "assign start & end points"
        // about the two objects it just removed.
        var a = Ensure("PathStart", root);
        var z = Ensure("PathEnd", root);
        a.transform.position = points[0];
        z.transform.position = points[points.Count - 1];
        b.startPoint = a.transform;
        b.endPoint = z.transform;

        b.river = line;
        b.useAuthoredSlots = false;      // no WORDSLOT_ objects here; this is not the island
        b.usePool = true;
        b.perspectiveCompensation = 0f;  // a walking player, not one fixed camera
        b.frameOnScreen = false;
        b.stoneScreenWidth = 0f;
        b.minFitFactor = 1f;
        b.curve = 0f;                    // the route is the shape; no meander on top
        b.stoneSideOffset = 0f;          // indoors the stones ARE the path, not beside it
        b.stoneScale = 0.5f;             // a corridor is not a river

        if (b.stoneA == null) b.stoneA = Stone("SM_WordStone_A");
        if (b.stoneB == null) b.stoneB = Stone("SM_WordStone_B");
        if (b.stoneC == null) b.stoneC = Stone("SM_WordStone_C");
        if (b.stoneKey == null) b.stoneKey = Stone("SM_WordStone_Key");

        // A SENTENCE TO LAY. The builder makes one stone per word and has no words
        // of its own — StoryBook only hands them over when a sentence is clicked at
        // runtime. Without this the route is correct, the prefabs are correct, and
        // absolutely nothing appears, which reads as "the stones are broken".
        if (string.IsNullOrWhiteSpace(b.sentence))
        {
            b.sentence = FirstSentence(book) ?? "The fox lived near a quiet river";
            log.Add($"sentence: none was set, so the stones show \"{b.sentence}\" until " +
                    $"one is clicked in the book");
        }

        EditorUtility.SetDirty(b);
        b.Build();
        log.Add($"stones: {b.Stones.Count} laid along the route, scale {b.stoneScale:0.00}" +
                (b.Stones.Count == 0
                    ? "  ← NONE. Check the stone prefabs are assigned on this component."
                    : ""));

        if (book != null)
        {
            Undo.RecordObject(book, "Build Reading Walk");
            book.stonePath = go.transform;
            book.rebuildStonesPerSentence = true;
            EditorUtility.SetDirty(book);
            log.Add("book wired to the stones — a sentence rebuilds them, one per word");
        }
    }

    /// <summary>The book's opening sentence, so the stones show something it owns.</summary>
    static string FirstSentence(StoryBook book)
    {
        if (book == null || string.IsNullOrWhiteSpace(book.paragraph)) return null;
        int stop = book.paragraph.IndexOfAny(new[] { '.', '!', '?' });
        string s = stop > 0 ? book.paragraph.Substring(0, stop) : book.paragraph;
        return s.Replace("*", "").Trim();          // drop the keyword markers
    }

    static GameObject Stone(string name) =>
        AssetDatabase.LoadAssetAtPath<GameObject>($"{STONE_DIR}/{name}.fbx");

    // ── helpers ─────────────────────────────────────────────────────────────

    static Vector3 RoomCentre(Bounds b) => Find("Volume Room 3")?.position ?? b.center;

    static bool Floor(Vector3 at, out float y, Transform ignore = null)
    {
        y = at.y;
        var hits = Physics.RaycastAll(at + Vector3.up * 3f, Vector3.down, 14f, ~0,
                                      QueryTriggerInteraction.Ignore);
        float best = float.NegativeInfinity;
        foreach (var h in hits)
        {
            if (ignore != null && (h.collider.transform == ignore ||
                                   h.collider.transform.IsChildOf(ignore))) continue;
            if (Mathf.Abs(h.normal.y) < 0.5f) continue;      // a wall is not a floor
            if (h.point.y > best) best = h.point.y;
        }
        if (best <= float.NegativeInfinity) return false;
        y = best;
        return true;
    }

    static Bounds WorldBounds(Transform t)
    {
        var rs = t.GetComponentsInChildren<Renderer>(false);
        if (rs.Length == 0) return new Bounds(t.position, Vector3.zero);
        var b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    static Transform FindLike(params string[] words)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                foreach (var w in words)
                    if (t.name.IndexOf(w, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return t;
        return null;
    }

    static Transform Find(string name)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t;
        return null;
    }

    static GameObject Ensure(string name, Transform parent)
    {
        var t = parent != null ? parent.Find(name) : null;
        if (t != null) return t.gameObject;
        var existing = parent == null ? Find(name) : null;
        if (existing != null) return existing.gameObject;

        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Build Reading Walk");
        return go;
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : Undo.AddComponent<T>(go);
    }

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v.sqrMagnitude > 1e-6f ? v.normalized : v; }
    static string V(Vector3 v) => $"({v.x:0.0}, {v.y:0.0}, {v.z:0.0})";
}
