// ===========================================================================
//  RiversideWalkSetup — one click: road, reader, walking camera (Editor)
// ===========================================================================
//  Tools > Great Library > Island > 6. Build Riverside Walk
//
//  With SC_IslandVista open this:
//    1. Puts the river back into WORLD space. The fixed-camera compensation
//       (screen-composed spacing, per-stone size solving) is turned OFF — a
//       camera that walks with the reader is what makes far stones readable
//       now, and leaving both on means the stones slide about as it moves.
//    2. Re-spaces the river so it can be WALKED: stones about two of their own
//       widths apart, the far end still at the shore, the near end extended out
//       to sea. A following camera has no reason to keep the whole river in one
//       frame, which is the freedom this is spending.
//    3. Lays a road along the far bank, through the dock, up to a reading mat
//       at the library entrance.
//    4. Drops in a placeholder reader and a walking camera.
//    5. Switches off the three things that would otherwise fight it for the
//       river and the camera: IslandFlow (flies the camera), WordPathGame
//       (clears the stones on Start) and StoryBook (rebuilds them on tap).
//
//  Everything it makes lives under one object, "RiversideWalk", so
//  "Remove Riverside Walk" is a clean undo. Safe to re-run.
// ===========================================================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RiversideWalkSetup
{
    const string ROOT = "RiversideWalk";
    const string MAT_DIR = "Assets/Art/Materials_Island";
    static readonly string[] SUSPEND = { "IslandFlow", "WordPathGame", "StoryBook" };

    [MenuItem("Tools/Great Library/Island/6. Build Riverside Walk")]
    public static void Build()
    {
        var path = Object.FindAnyObjectByType<WordPathBuilder>();
        if (path == null || path.startPoint == null || path.endPoint == null)
        {
            Debug.LogError("[Walk] No WordPathBuilder with start/end points in the " +
                           "open scene. Run Tools ▸ Great Library ▸ Build River Word " +
                           "Path first (with SC_IslandVista open).");
            return;
        }

        // ---------- 1. world-space river --------------------------------
        Undo.RecordObject(path, "Build Riverside Walk");
        path.perspectiveCompensation = 0f;   // no depth fudging: the camera comes to us
        path.frameOnScreen = false;          // no screen composition: the world is the shot
        path.stoneScreenWidth = 0f;
        path.minFitFactor = 1f;              // and no shrink-to-fit
        path.meanderWaves = 0.6f;
        path.curve = Mathf.Clamp(path.curve, 0.5f, 1.2f);

        // ---------- 2. re-space it for walking ---------------------------
        path.Build();
        float stoneWidth = MeasureStone(path);
        float spacing = Mathf.Clamp(stoneWidth * 1.9f, 1.6f, 4f);

        int words = WordPathBuilder.SplitWords(path.sentence).Count();
        float length = Mathf.Max(4f, spacing * Mathf.Max(1, words - 1));
        Vector3 dir = path.endPoint.position - path.startPoint.position;
        dir.y = 0f;
        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;

        Undo.RecordObject(path.startPoint, "Build Riverside Walk");
        path.startPoint.position = path.endPoint.position - dir * length;
        path.Build();
        WarnIfOffTheSea(path.startPoint.position);

        // ---------- 3. the scene furniture -------------------------------
        var root = Ensure(ROOT, null);
        root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        var entrance = FindByName("SOCKET_Entrance") ?? FindByName("DoorGlow");
        var dock = FindByName("SM_Dock");
        Transform lastStone = LastStone(path);

        var mat = Ensure("ReadingMat", root.transform);
        PlaceMat(mat, entrance, lastStone);

        var roadGO = Ensure("Road", root.transform);
        roadGO.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        var road = GetOrAdd<RiversideRoad>(roadGO);
        road.path = path;
        road.seat = mat.transform;
        // The dock is only a waypoint if it is actually on the way — its pivot can
        // sit anywhere in the FBX, and a stray one would send the reader on a
        // detour round the island.
        if (dock != null && lastStone != null &&
            Vector3.Distance(dock.position, lastStone.position) > 25f) dock = null;
        road.climbWaypoints = new List<Transform> { dock, entrance }
                              .Where(t => t != null).ToList();
        road.roadMaterial = MakeMaterial("M_Road", new Color(0.78f, 0.70f, 0.53f));
        road.Rebuild();

        if (road.StopCount == 0)
        {
            Debug.LogError("[Walk] The road found no stones to follow. Check that " +
                           "the word path built (stone prefabs assigned?).");
            return;
        }

        // ---------- 4. the reader and the camera -------------------------
        var readerGO = Ensure("Reader", root.transform);
        var actor = GetOrAdd<PlaceholderActor>(readerGO);
        var walker = GetOrAdd<PathWalker>(readerGO);
        walker.road = road;
        walker.path = path;
        walker.actor = actor;
        // Stood on the road, facing down it — so the preview frame below reads the
        // same way the first frame of Play does, rather than showing the reader
        // side-on at whatever rotation the last build left.
        float startD = Mathf.Max(0f, road.StopDistance(0) - walker.approach);
        readerGO.transform.position = road.PositionAt(startD);
        Vector3 heading = road.TangentAt(startD);
        heading.y = 0f;
        if (heading.sqrMagnitude > 0.0001f)
            readerGO.transform.rotation = Quaternion.LookRotation(heading.normalized, Vector3.up);

        var aim = Ensure("WalkAim", root.transform);
        var demoGO = Ensure("WalkDemo", root.transform);
        var demo = GetOrAdd<RiversideWalkDemo>(demoGO);
        demo.path = path; demo.road = road; demo.walker = walker;

        var camGO = GameObject.Find("Main Camera");
        if (camGO == null)
        {
            Debug.LogError("[Walk] No 'Main Camera' in the scene.");
            return;
        }
        var walkCam = GetOrAdd<WalkCamera>(camGO);
        walkCam.walker = walker;
        walkCam.aimTarget = aim.transform;
        Reframe(walkCam);
        var cam = camGO.GetComponent<Camera>();
        if (cam != null)
        {
            Undo.RecordObject(cam, "Build Riverside Walk");
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = Mathf.Max(cam.farClipPlane, 400f);
        }
        Compose(walkCam);

        // ---------- 5. stand the other drivers down ----------------------
        var stood = new List<string>();
        foreach (var n in SUSPEND)
        {
            var go = GameObject.Find(n);
            if (go == null || !go.activeSelf) continue;
            Undo.RecordObject(go, "Build Riverside Walk");
            go.SetActive(false);
            stood.Add(n);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = readerGO;

        Debug.Log(
            $"[Walk] Riverside walk built.\n" +
            $"  • river: {road.StopCount} stones, {spacing:0.0} m apart " +
            $"(stone ≈ {stoneWidth:0.0} m), {length:0.0} m long — " +
            $"PathStart moved to {path.startPoint.position}\n" +
            $"  • road: {road.Length:0.0} m from open water to the mat, " +
            $"{road.climbWaypoints.Count} climb waypoint(s)" +
            (dock == null ? " (no dock on the way)" : "") +
            (entrance == null ? " (no SOCKET_Entrance found)" : "") + "\n" +
            (stood.Count > 0 ? $"  • switched off while you test: {string.Join(", ", stood)}\n" : "") +
            $"  → the Game view is already on the opening frame; press Play to walk " +
            $"it. Tune the shot on Main Camera ▸ WalkCamera, the pace " +
            $"on Reader ▸ PathWalker.\n" +
            $"  → Cinemachine: untick WalkCamera ▸ Drive Camera, then Follow the " +
            $"Reader and LookAt '{aim.name}'.");
    }

    [MenuItem("Tools/Great Library/Island/6. Build Riverside Walk", true)]
    static bool BuildValidate() => !Application.isPlaying;

    /// <summary>
    /// Put the shot back to the authored composition. Worth its own menu item
    /// because the camera lives on Main Camera, not under ROOT — so it survives
    /// "Remove", and once its fields have been nudged about in the Inspector
    /// there is otherwise no way back short of deleting the component.
    /// </summary>
    [MenuItem("Tools/Great Library/Island/6b. Re-frame Walk Camera")]
    public static void ReframeCamera()
    {
        var camGO = GameObject.Find("Main Camera");
        var wc = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (wc == null)
        {
            Debug.LogError("[Walk] No WalkCamera on 'Main Camera'. Run " +
                           "Tools ▸ Great Library ▸ Island ▸ 6. Build Riverside Walk.");
            return;
        }
        Reframe(wc);
        Compose(wc);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = camGO;
        Debug.Log("[Walk] Shot reset and the camera parked on the opening frame — " +
                  "the Game view is now showing what Play will show. The rig widens " +
                  "and dollies out on its own for narrow (portrait) frames; one knob, " +
                  "Main Camera ▸ WalkCamera ▸ Shot Distance, moves all three shots.");
    }

    /// <summary>
    /// Park the camera on the opening shot. Without this the Game view keeps the
    /// old island-vista pose — which sits level with the river's first stone, so
    /// the near stones fall off the side of the frame and the layout gets blamed
    /// for what is really a camera that has not moved yet.
    /// </summary>
    static void Compose(WalkCamera wc)
    {
        Undo.RecordObject(wc.transform, "Frame Walk Camera");
        var cam = wc.GetComponent<Camera>();
        if (cam != null) Undo.RecordObject(cam, "Frame Walk Camera");
        wc.Compose();
        EditorUtility.SetDirty(wc.transform);
        if (cam != null) EditorUtility.SetDirty(cam);
    }

    /// <summary>The authored composition, in one place.</summary>
    static void Reframe(WalkCamera wc)
    {
        Undo.RecordObject(wc, "Re-frame Walk Camera");
        wc.travelBack = 4.2f; wc.travelUp = 2.5f; wc.travelSide = 2.2f; wc.travelFov = 42f;
        wc.readBack = 2.6f; wc.readUp = 1.7f; wc.readSide = 2.4f; wc.readFov = 34f;
        wc.seatFront = 5.0f; wc.seatUp = 2.6f; wc.seatSide = 1.6f; wc.seatFov = 45f;
        wc.lookAhead = 3.5f;

        wc.shotDistance = 1f;
        wc.fitToAspect = true;
        wc.referenceAspect = 16f / 9f;
        wc.aspectCompensation = 0.7f;
        wc.maxPullback = 2.4f;
        wc.maxFov = 64f;
        wc.minReaderDistance = 4f;
        EditorUtility.SetDirty(wc);
    }

    [MenuItem("Tools/Great Library/Island/Remove Riverside Walk")]
    public static void Remove()
    {
        var root = GameObject.Find(ROOT);
        if (root != null) Undo.DestroyObjectImmediate(root);

        var camGO = GameObject.Find("Main Camera");
        var wc = camGO != null ? camGO.GetComponent<WalkCamera>() : null;
        if (wc != null) Undo.DestroyObjectImmediate(wc);

        foreach (var n in SUSPEND)
        {
            var go = GameObject.Find(n);
            if (go != null && !go.activeSelf)
            {
                Undo.RecordObject(go, "Remove Riverside Walk");
                go.SetActive(true);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Walk] Riverside walk removed. The river is still laid out in " +
                  "world space — re-run Tools ▸ Great Library ▸ Build River Word Path " +
                  "to go back to the fixed-camera composition.");
    }

    // ── helpers ─────────────────────────────────────────────────────────────

    /// <summary>The mat: on the approach side of the door, facing back out to sea.</summary>
    static void PlaceMat(GameObject mat, Transform entrance, Transform lastStone)
    {
        Vector3 door = entrance != null ? entrance.position : new Vector3(0f, 2.6f, -0.4f);
        Vector3 outward = lastStone != null ? lastStone.position - door : Vector3.back;
        outward.y = 0f;
        outward = outward.sqrMagnitude > 0.01f ? outward.normalized : Vector3.back;

        mat.transform.SetPositionAndRotation(
            door + outward * 1.5f,
            Quaternion.LookRotation(outward, Vector3.up));   // the reader looks out

        // a flat disc; no collider, or it would eat taps meant for the stones
        var disc = mat.transform.Find("Disc");
        if (disc == null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Disc";
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(mat.transform, false);
            disc = go.transform;
        }
        disc.localPosition = new Vector3(0f, 0.02f, 0f);
        disc.localScale = new Vector3(1.5f, 0.02f, 1.5f);
        disc.GetComponent<MeshRenderer>().sharedMaterial =
            MakeMaterial("M_ReadingMat", new Color(0.70f, 0.28f, 0.26f));
    }

    /// <summary>A stone's world width, so the spacing is judged against the art.</summary>
    static float MeasureStone(WordPathBuilder path)
    {
        foreach (Transform c in path.transform)
        {
            var r = c.GetComponentInChildren<Renderer>();
            if (r != null) return Mathf.Max(r.bounds.size.x, r.bounds.size.z);
        }
        return 1.2f;
    }

    static Transform LastStone(WordPathBuilder path)
    {
        Transform last = null;
        foreach (Transform c in path.transform)
            if (c.GetComponent<WordStone>() != null) last = c;
        return last;
    }

    static void WarnIfOffTheSea(Vector3 p)
    {
        var sea = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None)
            .FirstOrDefault(r => r.name.ToLowerInvariant().Contains("sea") ||
                                 r.name.ToLowerInvariant().Contains("water"));
        if (sea == null) return;
        var b = sea.bounds;
        if (p.x >= b.min.x && p.x <= b.max.x && p.z >= b.min.z && p.z <= b.max.z) return;
        Debug.LogWarning($"[Walk] The river now starts at {p}, which is outside " +
                         $"'{sea.name}' ({b.min} … {b.max}). The first words will " +
                         "float over nothing — shorten the sentence, or scale the sea up.");
    }

    static Transform FindByName(string contains)
    {
        return Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude,
                                                   FindObjectsSortMode.None)
            .FirstOrDefault(t => t.name.Contains(contains));
    }

    static GameObject Ensure(string name, Transform parent)
    {
        var existing = parent == null ? GameObject.Find(name) : null;
        if (existing == null && parent != null)
        {
            var t = parent.Find(name);
            existing = t != null ? t.gameObject : null;
        }
        if (existing != null) return existing;

        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Build Riverside Walk");
        return go;
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = Undo.AddComponent<T>(go);
        return c;
    }

    static Material MakeMaterial(string name, Color c)
    {
        Directory.CreateDirectory(MAT_DIR);
        string p = $"{MAT_DIR}/{name}.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(p);
        if (m == null)
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            m = new Material(sh);
            AssetDatabase.CreateAsset(m, p);
        }
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.12f);
        EditorUtility.SetDirty(m);
        return m;
    }
}
