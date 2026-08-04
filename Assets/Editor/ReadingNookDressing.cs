// ===========================================================================
//  ReadingNookDressing — turn the bare plinth into a reading terrace (Editor)
// ===========================================================================
//  Tools > Great Library > Island >
//     9c. Dress The Reading Nook        — build it
//     9d. Undress The Reading Nook      — take it away, plinth back
//
//  RUN 9 FIRST. This dresses the nook that step 9 makes.
//
//  WHAT IT IS FOR. The book was standing on a white cylinder against open sea,
//  so the reading shot had nothing in it but a book, a reader and sky. This
//  spends the kit that is already imported (Assets/Prefabs/Kit) on the one shot
//  the player spends the most time looking at: a desk to read at, a chair, a
//  lit candle, clutter with a story in it, and — the part that actually fixes
//  the composition — SHELVES BEHIND THE BOOK, so the frame has a back wall
//  instead of a horizon.
//
//  NOTHING IS HARD-CODED TO A MODEL. Every piece is measured after it is
//  instantiated and then dropped so its base sits on the surface it belongs to
//  (the terrace, or the desktop). That way the layout survives the kit being
//  re-exported at a different scale, and it does not need this file to know how
//  big a chair is.
//
//  Everything lands under BookNook/Furniture, so 9d is a clean removal.
//
//  Bible Ch. 3.4 / 8.6 — lighting IS the progress bar. The candle prefabs carry
//  their own amber point lights (the import pipeline puts one on every
//  SOCKET_Flame), and this adds one warm reading light over the page so the
//  text is legible at night. Cool ambient, precious warm pools.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ReadingNookDressing
{
    const string KIT = "Assets/Prefabs/Kit";
    const string FURNITURE = "Furniture";

    /// <summary>Terrace pieces: prefab, position (metres, nook-local), yaw.</summary>
    /// Local axes: +Z is the way the BOOK FACES — away from the reader, so that is
    /// where the backdrop goes. The reader stands at -Z, slightly -X.
    static readonly (string prefab, Vector3 pos, float yaw)[] GROUND =
    {
        ("P_Desk_Reading",      new Vector3( 0.00f, 0f,  0.00f),   0f),
        ("P_Chair_A",           new Vector3( 0.95f, 0f, -0.75f),  25f),
        ("P_ShelfBay_A",        new Vector3(-1.45f, 0f,  2.35f),   0f),
        ("P_ShelfBay_A",        new Vector3( 1.45f, 0f,  2.35f),   0f),
        ("P_Column_A",          new Vector3(-2.25f, 0f,  0.35f),   0f),
        ("P_Column_A",          new Vector3( 2.25f, 0f,  0.35f),   0f),
        ("P_Scroll_Pile",       new Vector3(-1.30f, 0f,  0.55f),  30f),
        ("P_Globe_Desk",        new Vector3( 1.35f, 0f, -0.15f), -20f),
        ("P_Book_Stack",        new Vector3(-1.05f, 0f, -0.70f),  15f),
        ("P_Lexicon_Pedestal",  new Vector3( 2.05f, 0f,  1.60f),   0f),
    };

    /// <summary>Pieces a built room already has — skipped when the nook is indoors.</summary>
    static readonly HashSet<string> ROOM_SUPPLIES = new HashSet<string>
    {
        "P_ShelfBay_A", "P_Column_A", "P_Lexicon_Pedestal",
    };

    /// <summary>
    /// Desktop clutter: dropped onto the measured top of the desk. Kept clear of
    /// the book's own footprint — it is 0.80 m across the spread, so anything
    /// inside ±0.45 m of centre ends up sitting in the middle of the page.
    /// </summary>
    static readonly (string prefab, Vector3 pos, float yaw)[] ON_DESK =
    {
        ("P_Candle_Cluster_01", new Vector3( 0.54f, 0f,  0.20f),   0f),
        ("P_Inkpot",            new Vector3(-0.52f, 0f,  0.18f),   0f),
        ("P_Quill",             new Vector3(-0.44f, 0f,  0.02f),  35f),
    };

    // ======================================================================
    [MenuItem("Tools/Great Library/Island/9c. Dress The Reading Nook")]
    public static void Dress()
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station == null)
        {
            Debug.LogError("[Nook] No BookStation. Run Tools ▸ Great Library ▸ " +
                           "Island ▸ 9. Build Book Reading Loop first.");
            return;
        }
        var book = station.Book;
        var nook = station.transform;
        float groundY = nook.position.y;

        // ---------- a frame to lay the furniture out in --------------------
        // +Z along the way the book faces, so every number in the tables above
        // reads as "behind the book" / "beside the reader" instead of as a world
        // coordinate that stops meaning anything the moment the nook is dragged.
        var dress = Ensure(FURNITURE, nook);
        Vector3 facing = book.forward;
        facing.y = 0f;
        if (facing.sqrMagnitude < 1e-4f) facing = Vector3.forward;
        Undo.RecordObject(dress, "Dress Reading Nook");
        dress.SetPositionAndRotation(nook.position,
                                     Quaternion.LookRotation(facing.normalized, Vector3.up));
        Clear(dress);

        // ---------- the terrace --------------------------------------------
        // Indoors the room supplies its own floor, backdrop shelves and columns —
        // laying a second set inside them is how you get shelves standing in
        // shelves and two floors z-fighting.
        var missing = new List<string>();
        var floorSize = station.indoors ? Vector2Int.zero
                                        : TileFloor(dress, groundY, missing);

        GameObject desk = null;
        foreach (var (prefab, pos, yaw) in GROUND)
        {
            if (station.indoors && ROOM_SUPPLIES.Contains(prefab)) continue;
            var go = Place(prefab, dress, pos, yaw, groundY, missing);
            if (go != null && prefab == "P_Desk_Reading") desk = go;
        }

        // ---------- the desktop ---------------------------------------------
        float deskTop = groundY + 0.75f;                 // a sane desk if none loaded
        Vector3 deskSize = Vector3.zero;
        if (desk != null)
        {
            var db = Measure(desk);
            if (db.HasValue) { deskTop = db.Value.max.y; deskSize = db.Value.size; }
        }

        foreach (var (prefab, pos, yaw) in ON_DESK)
            Place(prefab, dress, pos, yaw, deskTop, missing);

        // ---------- the book, onto the desk ----------------------------------
        // Re-size it here rather than making the user re-place the whole nook: the
        // page has to be READ, so its size is the most likely thing to want changing.
        BookReadingLoopSetup.ScaleBook(book);

        Undo.RecordObject(book, "Dress Reading Nook");
        Vector3 seat = dress.TransformPoint(new Vector3(0f, 0f, -0.04f));
        book.position = new Vector3(seat.x, book.position.y, seat.z);
        var bb = Measure(book.gameObject);
        if (bb.HasValue) book.position += Vector3.up * (deskTop - bb.Value.min.y);

        // The reader has to stand clear of the desk they are reading across, so the
        // reading spot is pushed out by the desk's own size rather than by a guess.
        // The wider of the two footprint axes, because the measurement is a WORLD
        // box and the nook can be turned any way round.
        Undo.RecordObject(station, "Dress Reading Nook");
        station.standDistance =
            Mathf.Max(0.95f, Mathf.Max(deskSize.x, deskSize.z) * 0.5f + 0.55f);
        EditorUtility.SetDirty(station);

        // the plinth was scaffolding; the desk is the real thing
        var plinth = nook.Find("Pedestal");
        if (plinth != null) Undo.DestroyObjectImmediate(plinth.gameObject);

        ReadingLight(dress, deskTop);

        BookReadingLoopSetup.Recompose();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = dress.gameObject;

        Debug.Log(
            "[Nook] Reading terrace dressed.\n" +
            $"  • desk top at y = {deskTop:0.00} (desk measures {deskSize:0.00} m); " +
            $"the book now sits on it and the reader stands {station.standDistance:0.00} m out\n" +
            $"  • terrace floor {floorSize} tiles\n" +
            (missing.Count > 0
                ? $"  • NOT FOUND in {KIT}: {string.Join(", ", missing.Distinct())} — run " +
                  "Tools ▸ Great Library ▸ Build Kit Prefabs\n"
                : "  • every kit piece loaded\n") +
            "  • nudge anything under BookNook/Furniture; 9d removes the lot.\n" +
            "  • if a piece faces the wrong way, spin its Y — the kit's own forward " +
            "axis is whatever Blender exported and this cannot know it.");
    }

    [MenuItem("Tools/Great Library/Island/9c. Dress The Reading Nook", true)]
    static bool DressValidate() => !Application.isPlaying;

    // ======================================================================
    [MenuItem("Tools/Great Library/Island/9d. Undress The Reading Nook")]
    public static void Undress()
    {
        var station = Object.FindAnyObjectByType<BookStation>(FindObjectsInactive.Include);
        if (station == null) { Debug.LogError("[Nook] No BookStation."); return; }

        var dress = station.transform.Find(FURNITURE);
        if (dress != null) Undo.DestroyObjectImmediate(dress.gameObject);

        Undo.RecordObject(station, "Undress Reading Nook");
        station.standDistance = 0.95f;

        // Re-place rather than re-build: it is the step that rebuilds the plinth
        // AND puts the book back on top of it. A plain rebuild leaves the book
        // hanging in the air where the desktop used to be.
        BookReadingLoopSetup.ReplaceNook();
        Debug.Log("[Nook] Furniture removed; the book is back on its plinth.");
    }

    // ── placing ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Drop one kit piece so its BASE rests on <paramref name="baseY"/>. Measured,
    /// never assumed: a prefab's pivot may be at its base, its centre or wherever
    /// the Blender origin happened to be, and only the renderer bounds know.
    /// </summary>
    static GameObject Place(string prefabName, Transform parent, Vector3 localPos,
                            float yaw, float baseY, List<string> missing)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>($"{KIT}/{prefabName}.prefab");
        if (asset == null) { missing.Add(prefabName); return null; }

        var go = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        Undo.RegisterCreatedObjectUndo(go, "Dress Reading Nook");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        var b = Measure(go);
        if (b.HasValue) go.transform.position += Vector3.up * (baseY - b.Value.min.y);
        return go;
    }

    /// <summary>A tiled terrace under the whole nook, sized from the tile itself.</summary>
    static Vector2Int TileFloor(Transform parent, float groundY, List<string> missing)
    {
        const float WANT_X = 5.5f, WANT_Z = 6.0f;

        var asset = AssetDatabase.LoadAssetAtPath<GameObject>($"{KIT}/P_Floor_Tile_A.prefab");
        if (asset == null) { missing.Add("P_Floor_Tile_A"); return Vector2Int.zero; }

        // measure one tile, then throw it away — the grid below places the keepers
        var probe = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        var pb = Measure(probe);
        Vector3 tile = pb.HasValue ? pb.Value.size : Vector3.one;
        Object.DestroyImmediate(probe);
        if (tile.x < 0.05f || tile.z < 0.05f) return Vector2Int.zero;

        int nx = Mathf.Clamp(Mathf.CeilToInt(WANT_X / tile.x), 1, 8);
        int nz = Mathf.Clamp(Mathf.CeilToInt(WANT_Z / tile.z), 1, 8);

        var floor = new GameObject("Terrace");
        Undo.RegisterCreatedObjectUndo(floor, "Dress Reading Nook");
        floor.transform.SetParent(parent, false);

        for (int ix = 0; ix < nx; ix++)
            for (int iz = 0; iz < nz; iz++)
            {
                float x = (ix - (nx - 1) * 0.5f) * tile.x;
                float z = (iz - (nz - 1) * 0.5f) * tile.z + 0.9f;   // biased behind
                Place("P_Floor_Tile_A", floor.transform, new Vector3(x, 0f, z),
                      0f, groundY, missing);
            }
        return new Vector2Int(nx, nz);
    }

    /// <summary>
    /// One warm pool over the page. The candles carry their own lights, but they
    /// are short-range mood; this is the one that makes the TEXT readable, which is
    /// the whole job of this scene.
    /// </summary>
    static void ReadingLight(Transform parent, float deskTop)
    {
        var t = parent.Find("Light_Reading");
        GameObject go;
        if (t == null)
        {
            go = new GameObject("Light_Reading");
            Undo.RegisterCreatedObjectUndo(go, "Dress Reading Nook");
            go.transform.SetParent(parent, false);
        }
        else go = t.gameObject;

        go.transform.localPosition = new Vector3(0f, deskTop - parent.position.y + 1.15f, 0.15f);

        // explicit null check, not ??: Unity objects overload ==
        var l = go.GetComponent<Light>();
        if (l == null) l = Undo.AddComponent<Light>(go);
        l.type = LightType.Point;
        l.color = new Color(1f, 0.85f, 0.62f);
        l.intensity = 3.2f;
        l.range = 5.5f;
        l.shadows = LightShadows.None;         // mobile
    }

    // ── helpers ─────────────────────────────────────────────────────────────

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
        Undo.RegisterCreatedObjectUndo(go, "Dress Reading Nook");
        return go.transform;
    }

    static void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(t.GetChild(i).gameObject);
    }
}
