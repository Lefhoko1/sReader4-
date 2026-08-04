// ===========================================================================
//  LibraryHallBuilder — assemble a room from the modular kit (Editor)
// ===========================================================================
//  Tools > Great Library > Library >
//     1. Build Library Hall      — create (or rebuild) the hall
//     2. Hide Island Exterior    — switch the outdoor world off
//     2b. Show Island Exterior
//     Remove Library Hall
//
//  Also: select the LibraryHall object and use the Rebuild button in the
//  Inspector after turning any of its dials.
//
//  THE IDEA. A library is a BUILDING, not a pile of props on a hillside. The kit
//  was authored as a modular set precisely so rooms can be assembled from it, and
//  everything here follows from one measurement: the wall panel's own width is
//  the bay, and the bay is the grid. Floors, columns, beams, shelves and the
//  doorway are all placed on multiples of it, so the room is square and the
//  pieces meet by construction rather than by nudging.
//
//  PIVOTS ARE NOT TRUSTED. Every piece is instantiated, measured, then moved so
//  its BASE sits on the floor and its FOOTPRINT centres on the grid point it was
//  asked for. A Blender origin can be anywhere; bounds cannot lie.
//
//  Layout, in the hall's local space:
//      +Z is the back wall (where the shelves and the reading desk go)
//      -Z is the front wall (where the door goes)
//      the reader stands facing +Z, so the camera looks INTO the room
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LibraryHallBuilder
{
    const string KIT = "Assets/Prefabs/Kit";
    const string HALL = "LibraryHall";
    const string SHELL = "Shell";

    // The outdoor world, switched off rather than deleted — it is authored art and
    // "remove the island" should never mean "lose the island".
    static readonly string[] EXTERIOR =
        { "IslandWorld", "IslandVista", "SM_SkyBoard", "IslandWalk_Blender" };

    // ======================================================================
    [MenuItem("Tools/Great Library/Library/1. Build Library Hall")]
    public static void BuildMenu()
    {
        var hall = Object.FindAnyObjectByType<LibraryHall>(FindObjectsInactive.Include);
        if (hall == null)
        {
            var go = new GameObject(HALL);
            Undo.RegisterCreatedObjectUndo(go, "Build Library Hall");
            hall = Undo.AddComponent<LibraryHall>(go);
        }
        AutoFill(hall);
        Rebuild(hall);
        Selection.activeGameObject = hall.gameObject;
    }

    [MenuItem("Tools/Great Library/Library/1. Build Library Hall", true)]
    static bool BuildValidate() => !Application.isPlaying;

    /// <summary>Fill any empty prefab slot from the kit folder, by name.</summary>
    public static void AutoFill(LibraryHall h)
    {
        Undo.RecordObject(h, "Auto-fill Kit");
        if (h.floorA == null) h.floorA = Kit("P_Floor_Tile_A");
        if (h.floorB == null) h.floorB = Kit("P_Floor_Tile_B");
        if (h.wallFull == null) h.wallFull = Kit("P_Wall_Full_A");
        if (h.wallWindow == null) h.wallWindow = Kit("P_Wall_Window_A");
        if (h.wallRecess == null) h.wallRecess = Kit("P_Wall_Full_B_Recess");
        if (h.column == null) h.column = Kit("P_Column_A");
        if (h.arch == null) h.arch = Kit("P_Arch_Doorway");
        if (h.beam == null) h.beam = Kit("P_Ceiling_Beam");
        if (h.shelfBay == null) h.shelfBay = Kit("P_ShelfBay_A");
        if (h.deskLibrarian == null) h.deskLibrarian = Kit("P_Librarian_Desk");
        if (h.lanternHanging == null) h.lanternHanging = Kit("P_Lantern_Hanging");
        if (h.candles == null) h.candles = Kit("P_Candle_Cluster_01");
        EditorUtility.SetDirty(h);
    }

    static GameObject Kit(string n) =>
        AssetDatabase.LoadAssetAtPath<GameObject>($"{KIT}/{n}.prefab");

    // ======================================================================
    /// <summary>Tear the shell down and lay it out again from the dials.</summary>
    public static void Rebuild(LibraryHall h)
    {
        if (h.wallFull == null)
        {
            Debug.LogError("[Hall] No wall prefab. Run Tools ▸ Great Library ▸ " +
                           "Build Kit Prefabs, then Auto-fill.");
            return;
        }

        // ---------- the module ------------------------------------------
        // One measurement decides the whole room: the wall panel's own footprint.
        Vector3 wall = SizeOf(h.wallFull);
        bool widthAlongZ = wall.z > wall.x;
        float bay = Mathf.Max(wall.x, wall.z);
        float wallH = wall.y;
        if (bay < 0.2f || wallH < 0.2f)
        {
            Debug.LogError($"[Hall] The wall prefab measures {wall} — that cannot be " +
                           "a wall. Check the kit import scale.");
            return;
        }

        Undo.RecordObject(h, "Build Library Hall");
        h.bayWidth = bay;
        h.wallHeight = wallH;
        h.interior = new Vector2(h.Width, h.Depth);
        EditorUtility.SetDirty(h);

        // Panels whose width runs along Z need a quarter turn before any of the
        // placement below means what it says.
        float fix = (widthAlongZ ? 90f : 0f) + h.wallYawOffset + (h.flipWalls ? 180f : 0f);

        var shell = Ensure(SHELL, h.transform);
        Clear(shell);

        float halfX = h.Width * 0.5f, halfZ = h.Depth * 0.5f;
        var missing = new List<string>();
        int placed = 0;

        // ---------- floor -------------------------------------------------
        placed += Floor(h, shell, halfX, halfZ, missing);

        // ---------- walls ---------------------------------------------------
        // front (-Z) and back (+Z): panels run along X
        for (int i = 0; i < h.baysWide; i++)
        {
            float x = (i - (h.baysWide - 1) * 0.5f) * bay;
            bool isDoor = h.doorway && i == h.baysWide / 2;

            var front = isDoor ? h.arch : h.wallFull;
            if (Put(front, shell, new Vector3(x, 0f, -halfZ), fix, 0f, missing) != null) placed++;
            if (Put(h.wallFull, shell, new Vector3(x, 0f, halfZ), fix + 180f, 0f, missing) != null) placed++;
        }

        // sides (±X): panels run along Z, so a further quarter turn
        for (int j = 0; j < h.baysDeep; j++)
        {
            float z = (j - (h.baysDeep - 1) * 0.5f) * bay;
            bool win = h.wallWindow != null && h.windowEvery > 0 && j % h.windowEvery == 0;
            var panel = win ? h.wallWindow : h.wallFull;

            if (Put(panel, shell, new Vector3(-halfX, 0f, z), fix + 90f, 0f, missing) != null) placed++;
            if (Put(panel, shell, new Vector3(halfX, 0f, z), fix + 270f, 0f, missing) != null) placed++;
        }

        // ---------- columns, one bay in from each side wall -------------------
        if (h.columns && h.column != null && h.baysWide >= 3)
        {
            float cx = halfX - bay;
            for (int j = 1; j < h.baysDeep; j++)
            {
                float z = (j - h.baysDeep * 0.5f) * bay;
                if (Put(h.column, shell, new Vector3(-cx, 0f, z), 0f, 0f, missing) != null) placed++;
                if (Put(h.column, shell, new Vector3(cx, 0f, z), 0f, 0f, missing) != null) placed++;
            }
        }

        // ---------- beams across the ceiling ----------------------------------
        if (h.beams && h.beam != null)
        {
            Vector3 b = SizeOf(h.beam);
            float beamYaw = b.z > b.x ? 90f : 0f;       // lie it across the room
            for (int j = 1; j < h.baysDeep; j++)
            {
                float z = (j - h.baysDeep * 0.5f) * bay;
                if (Put(h.beam, shell, new Vector3(0f, 0f, z), beamYaw,
                        wallH - b.y, missing) != null) placed++;
            }
        }

        // ---------- shelves against the walls ---------------------------------
        if (h.shelves && h.shelfBay != null)
        {
            Vector3 s = SizeOf(h.shelfBay);
            float depth = Mathf.Min(s.x, s.z) * 0.5f + h.shelfInset;

            for (int j = 0; j < h.baysDeep; j++)
            {
                float z = (j - (h.baysDeep - 1) * 0.5f) * bay;
                if (h.windowEvery > 0 && j % h.windowEvery == 0) continue;   // not over a window
                if (Put(h.shelfBay, shell, new Vector3(-halfX + depth, 0f, z), 90f, 0f, missing) != null) placed++;
                if (Put(h.shelfBay, shell, new Vector3(halfX - depth, 0f, z), 270f, 0f, missing) != null) placed++;
            }
            // the back wall — the surface the reading shot looks straight at
            for (int i = 0; i < h.baysWide; i++)
            {
                float x = (i - (h.baysWide - 1) * 0.5f) * bay;
                if (Put(h.shelfBay, shell, new Vector3(x, 0f, halfZ - depth), 180f, 0f, missing) != null) placed++;
            }
        }

        // ---------- dressing ----------------------------------------------------
        if (h.librarianDesk && h.deskLibrarian != null)
            if (Put(h.deskLibrarian, shell, new Vector3(0f, 0f, halfZ - bay * 1.1f),
                    180f, 0f, missing) != null) placed++;

        if (h.lanterns && h.lanternHanging != null)
        {
            Vector3 l = SizeOf(h.lanternHanging);
            for (int j = 1; j < h.baysDeep; j += 2)
            {
                float z = (j - h.baysDeep * 0.5f) * bay;
                foreach (float x in new[] { -bay * 0.9f, bay * 0.9f })
                    if (Put(h.lanternHanging, shell, new Vector3(x, 0f, z), 0f,
                            wallH - l.y - 0.15f, missing) != null) placed++;
            }
        }

        // ---------- the reading nook, indoors -------------------------------------
        string nookNote = h.nookInside ? MoveNookInside(h, halfZ, bay) : "nook left where it was";

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(
            $"[Hall] Library hall built: {h.baysWide} x {h.baysDeep} bays = " +
            $"{h.Width:0.0} x {h.Depth:0.0} m interior, walls {wallH:0.0} m " +
            $"(one bay = {bay:0.00} m, measured off {h.wallFull.name}).\n" +
            $"  • {placed} pieces placed\n" +
            $"  • {nookNote}\n" +
            (missing.Count > 0
                ? $"  • MISSING prefabs: {string.Join(", ", missing.Distinct())}\n"
                : "") +
            "  • if you are looking at the OUTSIDE of the walls from inside, tick " +
            "LibraryHall ▸ Flip Walls and Rebuild.\n" +
            "  • Tools ▸ Great Library ▸ Library ▸ 2. Hide Island Exterior takes the " +
            "outdoor world away.");
    }

    /// <summary>Checkerboard floor, sized from the tile's own footprint.</summary>
    static int Floor(LibraryHall h, Transform shell, float halfX, float halfZ,
                     List<string> missing)
    {
        if (h.floorA == null) { missing.Add("P_Floor_Tile_A"); return 0; }

        Vector3 t = SizeOf(h.floorA);
        if (t.x < 0.05f || t.z < 0.05f) return 0;

        int nx = Mathf.Clamp(Mathf.CeilToInt(h.Width / t.x), 1, 40);
        int nz = Mathf.Clamp(Mathf.CeilToInt(h.Depth / t.z), 1, 40);

        var floor = Ensure("Floor", shell);
        int n = 0;
        for (int ix = 0; ix < nx; ix++)
            for (int iz = 0; iz < nz; iz++)
            {
                var use = (h.floorB != null && (ix + iz) % 2 == 1) ? h.floorB : h.floorA;
                float x = (ix - (nx - 1) * 0.5f) * t.x;
                float z = (iz - (nz - 1) * 0.5f) * t.z;
                if (Put(use, floor, new Vector3(x, 0f, z), 0f, 0f, missing) != null) n++;
            }
        return n;
    }

    /// <summary>
    /// Bring the reading desk indoors: near the back wall, book facing the shelves
    /// so the reader has their back to the door and the camera looks INTO the room.
    /// </summary>
    static string MoveNookInside(LibraryHall h, float halfZ, float bay)
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station == null) return "no BookStation to bring inside (run step 9 first)";

        var nook = station.transform;
        Undo.RecordObject(nook, "Build Library Hall");
        nook.position = h.transform.TransformPoint(
            new Vector3(0f, 0f, halfZ - bay * 2.6f));
        nook.rotation = h.transform.rotation;

        // The book's "up the page" is its forward, and the reader stands at the foot
        // of the page — so facing the book at +Z puts the reader on the door side.
        var book = station.Book;
        if (book != null)
        {
            Undo.RecordObject(book, "Build Library Hall");
            book.rotation = h.transform.rotation * Quaternion.Euler(-30f, 0f, 0f);
        }

        Undo.RecordObject(station, "Build Library Hall");
        station.indoors = true;                 // the hall already has a floor

        ReadingNookDressing.Dress();            // re-dress + re-compose on the new spot
        return $"nook moved indoors to {nook.position}, facing the back shelves";
    }

    // ======================================================================
    [MenuItem("Tools/Great Library/Library/2. Hide Island Exterior")]
    public static void HideExterior() => SetExterior(false);

    [MenuItem("Tools/Great Library/Library/2b. Show Island Exterior")]
    public static void ShowExterior() => SetExterior(true);

    // What HideExterior actually switched off. Recorded, because "show everything
    // in the list" is wrong: step 0 retires the old hand-placed island in favour of
    // the Blender one, and blindly re-activating would stand both of them up at once.
    const string HIDDEN_KEY = "GreatLibrary.HiddenExterior";

    static void SetExterior(bool on)
    {
        var touched = new List<string>();

        if (!on)
        {
            foreach (var name in EXTERIOR)
            {
                var go = Find(name);
                if (go == null || !go.activeSelf) continue;
                Undo.RecordObject(go, "Hide Island Exterior");
                go.SetActive(false);
                touched.Add(name);
            }
            EditorPrefs.SetString(HIDDEN_KEY, string.Join(",", touched));
        }
        else
        {
            foreach (var name in EditorPrefs.GetString(HIDDEN_KEY, "")
                                            .Split(',')
                                            .Where(s => s.Length > 0))
            {
                var go = Find(name);
                if (go == null || go.activeSelf) continue;
                Undo.RecordObject(go, "Show Island Exterior");
                go.SetActive(true);
                touched.Add(name);
            }
            EditorPrefs.DeleteKey(HIDDEN_KEY);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(touched.Count == 0
            ? $"[Hall] Nothing to {(on ? "show" : "hide")} — " +
              (on ? "this tool has nothing recorded as hidden."
                  : "the exterior is already off.")
            : $"[Hall] Island exterior {(on ? "shown" : "hidden")}: " +
              $"{string.Join(", ", touched)}.");
    }

    /// <summary>Find by name including inactive objects, which GameObject.Find won't.</summary>
    static GameObject Find(string name) =>
        Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,
                                            FindObjectsSortMode.None)
              .FirstOrDefault(t => t.name == name && t.parent == null)?.gameObject;

    // ======================================================================
    /// <summary>
    /// Stand the hall on the island, where the library always was: its DOORWAY on
    /// the authored entrance, the room running inland behind it, its floor resting
    /// on the terrain. That completes the loop the scene was always describing —
    /// walk the river, climb the stair, come through the door, read at the desk.
    /// </summary>
    [MenuItem("Tools/Great Library/Library/3. Stand Hall On The Island")]
    public static void StandOnIsland()
    {
        var hall = Object.FindAnyObjectByType<LibraryHall>(FindObjectsInactive.Include);
        if (hall == null)
        {
            Debug.LogError("[Hall] No LibraryHall. Run 1. Build Library Hall first.");
            return;
        }
        if (hall.bayWidth < 0.01f)
        {
            Debug.LogError("[Hall] The hall has never been built — press Rebuild Hall " +
                           "on it first, so it knows how big it is.");
            return;
        }

        SetExterior(true);      // the island and its sea come back

        // ---------- where the door goes ----------------------------------
        var road = Object.FindAnyObjectByType<RiversideRoad>();
        var path = Object.FindAnyObjectByType<WordPathBuilder>();

        Transform anchor = FindAny("SOCKET_Entrance") ?? FindAny("DoorGlow");
        Vector3 door;
        if (anchor != null) door = anchor.position;
        else if (road != null && road.seat != null) door = road.seat.position;
        else if (path != null && path.endPoint != null) door = path.endPoint.position;
        else
        {
            Debug.LogError("[Hall] Found nothing to stand the hall on — no " +
                           "SOCKET_Entrance, no reading mat, no river end point.");
            return;
        }

        // ---------- which way it faces -------------------------------------
        // Outward is the way the reader ARRIVES from: across the water, up the
        // stair. The door has to look at them.
        Vector3 outward = Vector3.zero;
        if (path != null && path.startPoint != null) outward = Flat(path.startPoint.position - door);
        if (outward.sqrMagnitude < 0.01f && road != null && road.StopCount > 0)
            outward = Flat(road.Stone(0) != null ? road.Stone(0).position - door : Vector3.zero);
        if (outward.sqrMagnitude < 0.01f && road != null && road.seat != null)
            outward = Flat(road.seat.forward);
        if (outward.sqrMagnitude < 0.01f) outward = Vector3.back;
        outward.Normalize();

        // The hall's front wall is at local -Z, so pointing its FORWARD inland puts
        // the doorway on the seaward face. The offset then turns the whole building
        // about the doorway, so the anchor still lands on the front wall whatever
        // angle you settle on.
        Quaternion rot = Quaternion.LookRotation(-outward, Vector3.up) *
                         Quaternion.Euler(0f, hall.doorYawOffset, 0f);
        Vector3 doorDir = -(rot * Vector3.forward);          // the way the front faces
        Vector3 centre = door - doorDir * (hall.Depth * 0.5f);

        // ---------- sit it on the ground -----------------------------------
        float y = GroundUnder(hall, centre, rot, out float spread, out int hits);

        Undo.RecordObject(hall.transform, "Stand Hall On The Island");
        hall.transform.SetPositionAndRotation(new Vector3(centre.x, y, centre.z), rot);

        // The nook is its own root object, not a child of the hall, so it does not
        // come along on its own — rebuilding re-homes it inside the moved room.
        Rebuild(hall);

        Selection.activeGameObject = hall.gameObject;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(
            $"[Hall] Hall stood on the island.\n" +
            $"  • door yaw offset {hall.doorYawOffset:0}° — if you are still looking " +
            "at the back of the building, run 3b. Turn Hall 90° until you are not\n" +
            $"  • doorway on {(anchor != null ? anchor.name : "the reading mat")} at " +
            $"{door:0.00}, room running inland, floor at y = {y:0.00}\n" +
            $"  • ground sampled at {hits}/5 points, {spread:0.00} m of fall across " +
            "the footprint" +
            (spread > 1.0f
                ? " — that is a lot for a flat floor. Expect the low corner to sit " +
                  "proud of the hill; either shrink Bays Deep or level that ground " +
                  "in Blender.\n"
                : ".\n") +
            (hits == 0
                ? "  • NOTHING was under the footprint with a collider, so the floor " +
                  "is at the doorway's own height — nudge the hall's Y if it floats.\n"
                : "") +
            "  • the river, the walk and the sea are all live again: the reader walks " +
            "the sentence in from the water, climbs to the door and reads inside.");
    }

    [MenuItem("Tools/Great Library/Library/3. Stand Hall On The Island", true)]
    static bool StandValidate() => !Application.isPlaying;

    /// <summary>
    /// Quarter-turn the building about its doorway. Which way an island's entrance
    /// faces is authored art, not something the river's direction can be trusted to
    /// imply, so this is the honest control: turn it until you can see the front,
    /// and the angle is remembered on the component.
    /// </summary>
    [MenuItem("Tools/Great Library/Library/3b. Turn Hall 90°")]
    public static void TurnHall()
    {
        var hall = Object.FindAnyObjectByType<LibraryHall>(FindObjectsInactive.Include);
        if (hall == null) { Debug.LogError("[Hall] No LibraryHall in the scene."); return; }

        Undo.RecordObject(hall, "Turn Hall");
        hall.doorYawOffset = Mathf.Repeat(hall.doorYawOffset + 90f, 360f);
        EditorUtility.SetDirty(hall);
        StandOnIsland();
    }

    [MenuItem("Tools/Great Library/Library/3b. Turn Hall 90°", true)]
    static bool TurnValidate() => !Application.isPlaying;

    /// <summary>
    /// The height to rest a flat floor at: the HIGHEST ground under the footprint,
    /// sampled at the centre and the four corners. Highest rather than average
    /// because a building standing slightly proud of a hill reads as a building,
    /// and one sunk into it reads as a bug.
    /// </summary>
    static float GroundUnder(LibraryHall hall, Vector3 centre, Quaternion rot,
                             out float spread, out int hits)
    {
        float hx = hall.Width * 0.5f, hz = hall.Depth * 0.5f;
        var samples = new[]
        {
            Vector3.zero,
            new Vector3(-hx, 0f, -hz), new Vector3(hx, 0f, -hz),
            new Vector3(-hx, 0f, hz), new Vector3(hx, 0f, hz),
        };

        // The hall's own pieces must not be what the ray lands on.
        bool wasActive = hall.gameObject.activeSelf;
        hall.gameObject.SetActive(false);

        float lo = float.MaxValue, hi = float.MinValue;
        hits = 0;
        foreach (var s in samples)
        {
            Vector3 p = centre + rot * s;
            if (!Physics.Raycast(new Vector3(p.x, centre.y + 60f, p.z), Vector3.down,
                                 out var hit, 200f, ~0, QueryTriggerInteraction.Ignore))
                continue;
            if (hit.collider.GetComponentInParent<WordStone>() != null) continue;
            hits++;
            lo = Mathf.Min(lo, hit.point.y);
            hi = Mathf.Max(hi, hit.point.y);
        }

        hall.gameObject.SetActive(wasActive);

        spread = hits > 0 ? hi - lo : 0f;
        return hits > 0 ? hi : centre.y;
    }

    /// <summary>
    /// Tell the walking camera what has to stay behind the reader. Without this the
    /// walk shots follow the road's tangent, and the island swings off frame every
    /// time the river bends — the library is the destination, so it is the one
    /// thing that should never leave the picture.
    /// </summary>
    static string WireBackdrop()
    {
        var camGO = GameObject.Find("Main Camera");
        var wc = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (wc == null) return "no WalkCamera — backdrop not wired";

        var mark = FindAny("SOCKET_HallCentre") ?? FindAny("SOCKET_Entrance");
        if (mark == null)
        {
            var lib = FindAny("SM_Library_Exterior");
            mark = lib != null ? lib : null;
        }
        if (mark == null) return "no library socket found — backdrop not wired";

        Undo.RecordObject(wc, "Wire Backdrop");
        wc.backdrop = mark;
        wc.backdropWeight = 0.85f;
        wc.backdropMinRange = 4f;
        wc.wordBias = 0.22f;

        // Back to the follow shot — it keeps the reader legible, which framing the
        // whole island never can on a phone — but with its heading HELD, so the
        // camera tracks their position and ignores which way they turn.
        wc.dioramaShot = false;
        wc.lockWalkDirection = true;
        wc.ResetWalkDirection();                // the world just moved under it

        // Kept tuned in case you switch the diorama back on from the Inspector.
        wc.dioramaFill = 0.85f;
        wc.dioramaPitch = 30f;
        wc.dioramaYaw = 0f;
        wc.dioramaFov = 50f;
        wc.dioramaLockRotation = true;
        wc.dioramaFollow = 0.5f;
        wc.dioramaZoom = 0.35f;
        wc.dioramaCharacterFrame = 5f;
        wc.ResetDioramaAngle();

        EditorUtility.SetDirty(wc);
        return $"camera backdrop = {mark.name}; follow shot with a locked heading " +
               "(tracks position, ignores the reader's turns)";
    }

    static Transform FindAny(string name) =>
        Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude,
                                            FindObjectsSortMode.None)
              .FirstOrDefault(t => t.name == name);

    static Vector3 Flat(Vector3 v) { v.y = 0f; return v; }

    // ======================================================================
    /// <summary>
    /// Put the sea back, the way the earlier scenes had it.
    ///
    /// Island ▸ 5. Build Sea Water bakes the wave maps and tunes M_Sea, but it only
    /// ever UPGRADES a sea that is already in the scene — the old hand-placed island
    /// carried its water as a child, so retiring it took the sea with it and there
    /// is nothing left for step 5 to find. This lays the plane down first, sized
    /// around the island and the river, and then hands over to the real pipeline so
    /// the material, the wave maps and the shader registration are identical to
    /// before rather than a second implementation.
    /// </summary>
    [MenuItem("Tools/Great Library/Library/4. Place The Sea")]
    public static void PlaceSea()
    {
        const float SEA_LEVEL = 0.03f;      // the height the island was authored to

        var existing = Object.FindObjectsByType<MeshRenderer>(
                               FindObjectsInactive.Include, FindObjectsSortMode.None)
                           .FirstOrDefault(IsSeaish);

        if (existing != null)
        {
            if (!existing.gameObject.activeSelf)
            {
                Undo.RecordObject(existing.gameObject, "Place The Sea");
                existing.gameObject.SetActive(true);
            }
            Debug.Log($"[Sea] '{existing.name}' is already the sea — re-running the " +
                      "wave pipeline over it rather than laying a second one.");
        }
        else
        {
            // ---- how far it has to reach --------------------------------
            var b = new Bounds(Vector3.zero, Vector3.one * 60f);
            var island = Find("IslandWorld") ?? Find("IslandVista");
            if (island != null)
            {
                var rends = island.GetComponentsInChildren<Renderer>(true)
                                  .Where(r => !(r is ParticleSystemRenderer)).ToArray();
                if (rends.Length > 0)
                {
                    b = rends[0].bounds;
                    foreach (var r in rends) b.Encapsulate(r.bounds);
                }
            }
            var path = Object.FindAnyObjectByType<WordPathBuilder>();
            if (path != null && path.startPoint != null) b.Encapsulate(path.startPoint.position);
            if (path != null && path.endPoint != null) b.Encapsulate(path.endPoint.position);

            // Generous: the horizon has to be water, not the edge of a plane.
            float span = Mathf.Clamp(Mathf.Max(b.size.x, b.size.z) * 6f, 200f, 2000f);

            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "SM_Sea";                       // the pipeline finds it by name
            Undo.RegisterCreatedObjectUndo(go, "Place The Sea");

            // A sea-sized collider would swallow every ground raycast in the project
            // — the hall's footing, the book nook's, the camera's obstruction test.
            var col = go.GetComponent<Collider>();
            if (col != null) Undo.DestroyObjectImmediate(col);

            go.transform.position = new Vector3(b.center.x, SEA_LEVEL, b.center.z);
            go.transform.localScale = Vector3.one * (span / 10f);   // Unity's plane is 10 m
            go.GetComponent<MeshRenderer>().shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;

            Debug.Log($"[Sea] Laid a {span:0} x {span:0} m sea at y = {SEA_LEVEL:0.00}, " +
                      $"centred on {b.center:0.0}. If the shoreline sits wrong, nudge " +
                      "SM_Sea's Y — that one number is the water line.");
        }

        // the real thing: wave maps, M_Sea on the stylized shader, dense grid
        SeaWaterSetup.BuildSeaWater();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Tools/Great Library/Library/4. Place The Sea", true)]
    static bool SeaValidate() => !Application.isPlaying;

    static bool IsSeaish(MeshRenderer r)
    {
        return Has(r.gameObject.name) ||
               r.sharedMaterials.Any(m => m != null && Has(m.name));

        bool Has(string s) =>
            s.IndexOf("sea", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.IndexOf("water", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.IndexOf("ocean", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    // ======================================================================
    /// <summary>
    /// Move the reading nook inside the library that came from Blender, onto its
    /// authored SOCKET_ReadingDesk, and wire the doorway and hall-centre sockets so
    /// the reader walks in through the door under a wide shot of the room.
    ///
    /// The facing is derived from the two sockets rather than from any axis: the
    /// line from the entrance to the reading desk IS "further into the room", and
    /// the book's forward is its top-of-page, so pointing the book that way puts
    /// the reader on the door side with the shelf wall behind the page. That holds
    /// however the FBX axis conversion came out.
    /// </summary>
    [MenuItem("Tools/Great Library/Library/5. Move Nook Into The Blender Library")]
    public static void NookIntoBlenderLibrary()
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station == null)
        {
            Debug.LogError("[Hall] No BookStation. Run Island ▸ 9. Build Book Reading Loop.");
            return;
        }

        var desk  = FindAny("SOCKET_ReadingDesk");
        var entry = FindAny("SOCKET_Entrance");
        var centre = FindAny("SOCKET_HallCentre");
        if (desk == null || entry == null)
        {
            Debug.LogError("[Hall] SOCKET_ReadingDesk / SOCKET_Entrance not in the " +
                           "scene. Run Island ▸ 0. Rebuild World From Blender so the " +
                           "new island FBX (which carries them) is instantiated.");
            return;
        }

        Vector3 inward = Flat(desk.position - entry.position);
        if (inward.sqrMagnitude < 1e-4f) inward = Vector3.forward;
        inward.Normalize();

        var nook = station.transform;
        Undo.RecordObject(nook, "Nook Into Library");
        nook.SetPositionAndRotation(desk.position,
                                    Quaternion.LookRotation(inward, Vector3.up));

        var book = station.Book;
        if (book != null)
        {
            Undo.RecordObject(book, "Nook Into Library");
            book.rotation = Quaternion.LookRotation(inward, Vector3.up) *
                            Quaternion.Euler(-30f, 0f, 0f);
        }

        Undo.RecordObject(station, "Nook Into Library");
        station.indoors = true;                 // the Blender hall has its own floor
        station.doorway = entry;
        station.hallFocus = centre != null ? centre : desk;
        EditorUtility.SetDirty(station);

        // The Unity-side modular hall is what this replaces.
        var hall = Object.FindAnyObjectByType<LibraryHall>(FindObjectsInactive.Include);
        if (hall != null)
        {
            Undo.DestroyObjectImmediate(hall.gameObject);
            Debug.Log("[Hall] Removed the Unity-side modular hall — the Blender " +
                      "library replaces it.");
        }

        // The Blender hall already HAS a desk, chair, globe and clutter. Running the
        // Unity dressing pass here would stand a second desk inside the first, so
        // strip it and let the book sit on the furniture that is already there.
        var dressed = nook.Find("Furniture");
        if (dressed != null)
        {
            Undo.DestroyObjectImmediate(dressed.gameObject);
            Debug.Log("[Hall] Removed the Unity nook furniture — the Blender hall " +
                      "supplies the desk.");
        }

        string seat = "no desk mesh found; book left at socket height";
        var blenderDesk = NearestNamed("Desk_Reading", desk.position, 4f);
        if (blenderDesk != null && book != null)
        {
            BookReadingLoopSetup.ScaleBook(book);
            var db = Measure(blenderDesk.gameObject);
            var bb = Measure(book.gameObject);
            if (db.HasValue && bb.HasValue)
            {
                book.position += new Vector3(db.Value.center.x - bb.Value.center.x,
                                             db.Value.max.y - bb.Value.min.y,
                                             db.Value.center.z - bb.Value.center.z);
                Undo.RecordObject(station, "Nook Into Library");
                station.standDistance = Mathf.Max(
                    0.95f, Mathf.Max(db.Value.size.x, db.Value.size.z) * 0.5f + 0.55f);
                seat = $"book seated on '{blenderDesk.name}' at y {db.Value.max.y:0.00}";
            }
        }

        string backdrop = WireBackdrop();
        BookReadingLoopSetup.Recompose();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = nook.gameObject;
        Debug.Log(
            "[Hall] Reading nook moved inside the Blender library.\n" +
            $"  • desk on {desk.name} at {desk.position:0.00}, facing further into " +
            $"the room ({inward:0.00}) so the shelf wall sits behind the page\n" +
            $"  • {seat}\n" +
            $"  • {backdrop}\n" +
            $"  • doorway = {entry.name}" +
            (centre != null ? $", hall shot = {centre.name}" : " (no SOCKET_HallCentre " +
             "— the reveal will frame the desk instead of the room)") + "\n" +
            "  • the reader now STARTS outside the door and walks in under a wide " +
            "shot of the hall; tune it on BookFlow ▸ Reveal Seconds / Reveal Fill.");
    }

    [MenuItem("Tools/Great Library/Library/5. Move Nook Into The Blender Library", true)]
    static bool NookValidate() => !Application.isPlaying;

    // ======================================================================
    /// <summary>
    /// Make the reader walk the path that was actually authored in Blender.
    ///
    /// Two things put them beside it instead of on it. First, the road picks which
    /// BANK of the stone line to run along, and step 0 forces `side = 1` — if the
    /// authored path is on the other side, the reader walks the river on the wrong
    /// hand and then cuts across to reach the stair. Second, the walk polyline now
    /// runs from the last stone all the way to the desk, so feeding all of it in as
    /// "the climb" sends them back out over the water before they turn around.
    ///
    /// Both are answered from the data rather than by taste: the side is whichever
    /// one the authored path is on, and the climb starts where the stones stop.
    /// </summary>
    [MenuItem("Tools/Great Library/Library/6. Fit The Walk To The Blender Path")]
    public static void FitWalkToBlenderPath()
    {
        var road = Object.FindAnyObjectByType<RiversideRoad>(FindObjectsInactive.Include);
        if (road == null) { Debug.LogError("[Walk] No RiversideRoad."); return; }
        // In the editor the river is usually empty — the book lays it out at run
        // time, one sentence at a time. Rather than refuse, lay the builder's own
        // sentence so there is something real to measure the bank against.
        var builder = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (road.StopCount == 0) road.Rebuild();
        if (road.StopCount == 0 && builder != null)
        {
            builder.Build();
            road.Rebuild();
            Debug.Log("[Walk] The river was empty, so the builder's own sentence was " +
                      "laid out to measure against — the book replaces it on Play.");
        }
        if (road.StopCount == 0)
        {
            Debug.LogError("[Walk] No stones and no WordPathBuilder to make any. Run " +
                           "Tools ▸ Great Library ▸ Build River Word Path first.");
            return;
        }

        var walkGO = FindAny("IslandWalk_Blender");
        var line = walkGO != null ? walkGO.GetComponent<WordRiverPath>() : null;
        if (line == null || line.points == null || line.points.Length < 2)
        {
            Debug.LogError("[Walk] No IslandWalk_Blender polyline. Run Tools ▸ Great " +
                           "Library ▸ Island ▸ 8. Import Island Walkway From Blender.");
            return;
        }

        // ---- direction of travel, from the stones themselves ---------------
        Vector3 first = road.Stone(0).position;
        Vector3 last = road.Stone(road.StopCount - 1).position;
        Vector3 dir = Flat(last - first);
        if (dir.sqrMagnitude < 1e-4f) dir = Vector3.forward;
        dir.Normalize();
        Vector3 lateral = Vector3.Cross(Vector3.up, dir).normalized;

        // ---- keep only the part of the walk that is AHEAD of the last stone -
        var climb = line.points.Where(p => Vector3.Dot(Flat(p - last), dir) > -0.25f)
                               .ToList();
        int dropped = line.points.Length - climb.Count;
        if (climb.Count < 2)
        {
            Debug.LogError("[Walk] The authored walk has no points past the last " +
                           "stone — is the river laid out where Blender put it? " +
                           "Re-run Island ▸ 7 and 8.");
            return;
        }

        // ---- which bank is the authored path on? ---------------------------
        // Sample the first few climb points: their average lateral offset from the
        // stone line is the side the art expects the reader on.
        float lat = climb.Take(Mathf.Min(4, climb.Count))
                         .Average(p => Vector3.Dot(Flat(p - last), lateral));
        int side = Mathf.Abs(lat) < 0.15f ? road.side : (lat < 0f ? -1 : 1);

        Undo.RecordObject(road, "Fit Walk To Blender Path");
        road.side = side;
        road.drawMesh = false;                 // the Blender slabs ARE the path now

        var host = Ensure("ClimbPoints", road.transform);
        Clear(host);
        var waypoints = new List<Transform>();
        for (int i = 0; i < climb.Count; i++)
        {
            var go = new GameObject($"Climb_{i:00}");
            Undo.RegisterCreatedObjectUndo(go, "Fit Walk To Blender Path");
            go.transform.SetParent(host, false);
            go.transform.position = climb[i];
            waypoints.Add(go.transform);
        }

        // The road ends at the door; BookWalkFlow takes the reader in from there.
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        Transform seat = null;
        if (station != null && station.doorway != null)
        {
            var s = Ensure("RoadSeat", road.transform);
            s.position = station.doorway.position;
            s.rotation = Quaternion.LookRotation(
                Flat(station.LookAt - station.doorway.position).normalized, Vector3.up);
            seat = s;
            waypoints.RemoveAll(t => Vector3.Distance(t.position, s.position) < 0.6f);
        }
        road.climbWaypoints = waypoints;
        if (seat != null) road.seat = seat;
        road.Rebuild();
        string backdrop = WireBackdrop();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = road.gameObject;
        Debug.Log(
            "[Walk] Road fitted to the Blender path.\n" +
            $"  • bank: side = {side} (authored path sits {lat:+0.00;-0.00} m " +
            $"{(lat < 0 ? "left" : "right")} of the stone line)\n" +
            $"  • climb: {waypoints.Count} authored waypoints, {dropped} seaward " +
            "point(s) dropped so the walk stops doubling back over the water\n" +
            (seat != null
                ? "  • the road now ends at the doorway; the reading flow walks them " +
                  "in from there\n"
                : "  • no BookStation doorway — the road still ends at its old seat\n") +
            $"  • {backdrop}\n" +
            $"  • {road.StopCount} stops over {road.Length:0.0} m");
    }

    [MenuItem("Tools/Great Library/Library/6. Fit The Walk To The Blender Path", true)]
    static bool FitValidate() => !Application.isPlaying;

    // ======================================================================
    [MenuItem("Tools/Great Library/Library/Remove Library Hall")]
    public static void Remove()
    {
        var hall = Object.FindAnyObjectByType<LibraryHall>(FindObjectsInactive.Include);
        if (hall != null) Undo.DestroyObjectImmediate(hall.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Hall] Library hall removed.");
    }

    // ── placing ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Put one piece on the grid: FOOTPRINT centred on <paramref name="localPos"/>,
    /// BASE resting at <paramref name="baseY"/>. Both are measured, because a kit
    /// piece's pivot may sit anywhere its author left it and a room assembled from
    /// pivots has gaps in it.
    /// </summary>
    static GameObject Put(GameObject prefab, Transform parent, Vector3 localPos,
                          float yaw, float baseY, List<string> missing)
    {
        if (prefab == null) { missing.Add("(unassigned slot)"); return null; }

        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(go, "Build Library Hall");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        var b = Measure(go);
        if (b.HasValue)
        {
            Vector3 want = parent.TransformPoint(localPos);
            Vector3 fix = new Vector3(want.x - b.Value.center.x,
                                      baseY + parent.position.y - b.Value.min.y,
                                      want.z - b.Value.center.z);
            go.transform.position += fix;
        }
        return go;
    }

    /// <summary>A prefab's size, measured off the asset without touching the scene.</summary>
    static Vector3 SizeOf(GameObject prefab)
    {
        if (prefab == null) return Vector3.zero;
        var probe = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        var b = Measure(probe);
        Object.DestroyImmediate(probe);
        return b.HasValue ? b.Value.size : Vector3.zero;
    }

    /// <summary>
    /// The nearest active object called <paramref name="name"/> within
    /// <paramref name="radius"/> of a point. Name alone is not enough — the kit
    /// piece is instanced all over the world and only the one standing at this
    /// socket is the desk we mean.
    /// </summary>
    static Transform NearestNamed(string name, Vector3 near, float radius)
    {
        Transform best = null;
        float bestD = radius * radius;
        foreach (var t in Object.FindObjectsByType<Transform>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (!t.name.StartsWith(name)) continue;
            float d = (t.position - near).sqrMagnitude;
            if (d < bestD) { bestD = d; best = t; }
        }
        return best;
    }

    static Bounds? Measure(GameObject go)
    {
        var rends = go.GetComponentsInChildren<Renderer>(true)
                      .Where(r => !(r is ParticleSystemRenderer)).ToArray();
        if (rends.Length == 0) return null;
        var b = rends[0].bounds;
        foreach (var r in rends) b.Encapsulate(r.bounds);
        return b;
    }

    static Transform Ensure(string name, Transform parent)
    {
        var t = parent.Find(name);
        if (t != null) return t;
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Build Library Hall");
        return go.transform;
    }

    static void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(t.GetChild(i).gameObject);
    }
}

// ===========================================================================
//  The Inspector: dials plus the two buttons that matter.
// ===========================================================================
[CustomEditor(typeof(LibraryHall))]
public class LibraryHallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var hall = (LibraryHall)target;

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            if (GUILayout.Button("Rebuild Hall", GUILayout.Height(28)))
                LibraryHallBuilder.Rebuild(hall);
            if (GUILayout.Button("Auto-fill Kit Prefabs"))
                LibraryHallBuilder.AutoFill(hall);
        }
        EditorGUILayout.HelpBox(
            "Turn a dial, then Rebuild. If you can see the OUTSIDE of the walls " +
            "from inside the room, tick Flip Walls — wall meshes are one-sided and " +
            "which way they face is the kit's decision, not something this can " +
            "measure.", MessageType.Info);
    }
}
