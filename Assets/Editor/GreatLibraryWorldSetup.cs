// ===========================================================================
//  GreatLibraryWorldSetup — put the Blender world in charge of placement
// ===========================================================================
//  Tools > Great Library > Island > 0. Rebuild World From Blender
//
//  WHY THIS EXISTS. The island was in the scene as SM_Island_Vista.fbx placed by
//  hand at position (-8.33, 0, 10.42) and rotated 154.57° about Y. Everything
//  authored in Blender — the river curve, the walkway, the SLOT markers — is in
//  Blender coordinates, and WordRiverImport can only solve a TRANSLATION plus one
//  of four axis flips (0° and 180° yaws). There is no candidate that can express
//  154.57°, so the importer picked the nearest (180°), buried what it could in the
//  offset, and left ~25° of unmodelled yaw. That is metres of drift by the far end
//  of the river: stones that look nearly right by the dock and a reader who ends
//  up standing in the sea.
//
//  The fix is not a better solver. It is to stop having two coordinate systems:
//  bring the whole world in as ONE root at identity, and Blender coordinates
//  simply ARE Unity coordinates. Every authored thing then agrees with every
//  other authored thing by construction, and the JSON importers go exact
//  (residual ≈ 0) instead of approximate.
//
//  WHAT IT DOES, in order — each step is re-runnable and none of it deletes:
//    1. Instantiates SM_Island_World.fbx at identity, and switches the old
//       hand-placed island OFF (never destroys it — 0b puts it back).
//    2. Re-runs both Blender importers, now against the world at identity, and
//       reports the residual so you can SEE that it is exact.
//    3. Retires the generated sand ribbon: the road becomes a route, the reader
//       walks the stone line at sea and the authored stair ashore.
//    4. Re-stands the reader and parks the camera on the opening frame.
//
//  MATERIALS. The retired island carries whatever was tweaked on it in Unity —
//  notably the animated water shader. The new root arrives with the materials the
//  FBX ships. Step 1 says so in the log rather than guessing which you want.
// ===========================================================================
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GreatLibraryWorldSetup
{
    const string WORLD_FBX = "Assets/Art/Models_Island/SM_Island_World.fbx";
    const string OLD_FBX = "Assets/Art/Models_Island/SM_Island_Vista.fbx";
    const string WORLD_GO = "IslandWorld";
    const string WALK_GO = "IslandWalk_Blender";
    const string WALK_POINTS = "WalkPoints";

    /// <summary>
    /// How far the word stones sit off the walking line, in metres. About one
    /// stone's width: clear of the reader's feet, still close enough to read
    /// without the eye having to leave the path.
    /// </summary>
    const float STONE_ASIDE = 0.9f;

    [MenuItem("Tools/Great Library/Island/0. Rebuild World From Blender", true)]
    static bool BuildValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/0. Rebuild World From Blender")]
    public static void Build()
    {
        var log = new List<string>();

        // ---------- 1. the world, as one root at identity -------------------
        var world = PlaceWorld(log);
        if (world == null) return;
        int retired = RetireOldIsland(log);

        // ---------- 2. the authored lines, now solvable exactly -------------
        // Order matters: the landmarks these solve against live under the new
        // root, so it has to be in the scene and active before either runs.
        WordRiverImport.Import();
        WordRiverImport.ImportWalk();
        log.Add("re-ran both Blender importers — check their residuals above; " +
                "they should read '— exact' now, not 'SUSPECT'.");

        // ---------- 3. the route, not a ribbon ------------------------------
        // Inactive included on purpose: 'Remove Riverside Walk' leaves the walk in
        // the scene switched off, and a road this cannot see is a road this
        // silently fails to fix.
        var road = Object.FindAnyObjectByType<RiversideRoad>(FindObjectsInactive.Include);
        if (road == null)
        {
            Debug.LogError(
                "[World] Steps 1–2 done, but there is no RiversideRoad in the scene, " +
                "so the stones and the reader were NOT touched — this is the step " +
                "that slides the words off the path and stands the reader on top of " +
                "it. Run 'Tools ▸ Great Library ▸ Island ▸ 6. Build Riverside Walk', " +
                "then run this again.\n  • " + string.Join("\n  • ", log));
            return;
        }
        if (!road.gameObject.activeInHierarchy)
            log.Add($"NOTE: '{road.name}' is switched off in the scene — it was still " +
                    $"set up, but nothing will walk until you enable it.");
        Route(road, log);

        // ---------- 4. the reader and the shot ------------------------------
        if (road.StopCount > 0)
        {
            log.Add(StandReader(road));
            RiversideWalkSetup.ReframeCamera();
            log.Add("camera re-framed and parked on the opening frame.");
        }
        else
        {
            log.Add("NOTE: the road found no stones, so the reader and camera were " +
                    "left alone. Run '6. Build Riverside Walk' once the river is up.");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = world;

        Debug.Log("[World] Rebuilt from Blender.\n  • " +
                  string.Join("\n  • ", log) +
                  (retired > 0
                      ? "\n  ! The retired island still holds any material work done in " +
                        "Unity (the animated water shader). The new root ships the FBX's " +
                        "own materials. Re-apply what you want, or run 0b to go back."
                      : ""));
    }

    /// <summary>
    /// Put SM_Island_World.fbx in the scene at identity. Identity is the whole
    /// point: it is what makes a Blender coordinate and a Unity coordinate the
    /// same number, so nothing downstream has to solve for the difference.
    /// </summary>
    static GameObject PlaceWorld(List<string> log)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(WORLD_FBX);
        if (asset == null)
        {
            Debug.LogError($"[World] {WORLD_FBX} not found. Re-run the Blender " +
                           "whole-world export first.");
            return null;
        }

        // Inactive included: 0b leaves this root switched off, and GameObject.Find
        // would not see it — so a second Build would quietly grow a second world.
        var go = FindAnywhere(WORLD_GO);
        if (go == null)
        {
            go = (GameObject)PrefabUtility.InstantiatePrefab(asset);
            go.name = WORLD_GO;
            Undo.RegisterCreatedObjectUndo(go, "Rebuild World From Blender");
            log.Add($"instantiated {WORLD_FBX} as '{WORLD_GO}'.");
        }
        else log.Add($"'{WORLD_GO}' was already in the scene — reused.");

        Undo.RecordObject(go.transform, "Rebuild World From Blender");
        go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        go.transform.localScale = Vector3.one;
        go.SetActive(true);

        int slots = go.GetComponentsInChildren<Transform>(true)
                      .Count(t => t.name.StartsWith("SLOT_"));
        log.Add($"world root at identity: {go.transform.childCount} top-level " +
                $"object(s), {slots} authored SLOT marker(s).");
        return go;
    }

    /// <summary>
    /// Switch off every instance of the old hand-placed island. Switched off and
    /// not destroyed on purpose: it is the only copy of whatever was tuned on it
    /// in Unity, and being able to flip between the two is how you tell whether
    /// the new one actually landed right.
    /// </summary>
    static int RetireOldIsland(List<string> log)
    {
        int n = 0;
        foreach (var go in InstancesOf(OLD_FBX))
        {
            if (!go.activeSelf) continue;
            Undo.RecordObject(go, "Rebuild World From Blender");
            go.SetActive(false);
            n++;
            log.Add($"retired the old hand-placed island '{go.name}' " +
                    $"(was at {go.transform.position}, " +
                    $"yaw {go.transform.eulerAngles.y:0.00}°) — switched off, not deleted.");
        }
        if (n == 0) log.Add("no active old island instance found — nothing to retire.");
        return n;
    }

    /// <summary>
    /// The walk becomes a route rather than a built surface: no ribbon, no lateral
    /// offset out at sea (the stones ARE the path there — it is what stepping
    /// stones are for), and the climb follows the stair modelled into the island
    /// instead of a curve invented to reach the door.
    /// </summary>
    static void Route(RiversideRoad road, List<string> log)
    {
        Undo.RecordObject(road, "Rebuild World From Blender");
        road.drawMesh = false;

        // The word stones step aside; the reader keeps the line.
        //
        // Blender ships the path itself — twenty SLOT_ stepping stones laid along
        // the river — and the builder puts a word stone on that same curve, so the
        // words land ON the path stones. Sliding the words off the line leaves the
        // authored stones clear to walk on and the words readable beside them.
        //
        // The road is derived from the WORD stones, so it would follow them
        // sideways. Cancelling the same distance back (the builder's lateral and
        // the road's are opposite conventions, which is why side is +1 here) puts
        // the route exactly back on the authored line — on top of the path stones,
        // which is where a person walks.
        float aside = STONE_ASIDE;
        var builder = Object.FindAnyObjectByType<WordPathBuilder>(FindObjectsInactive.Include);
        if (builder != null)
        {
            Undo.RecordObject(builder, "Rebuild World From Blender");
            builder.stoneSideOffset = aside;
            builder.Build();
            log.Add($"word stones slid {aside:0.00} m off the path line " +
                    $"(WordPathBuilder ▸ Stone Side Offset — negate it to put them " +
                    $"on the other side).");
        }
        else
        {
            aside = 0f;
            log.Add("NOTE: no WordPathBuilder found, so the words were not moved.");
        }

        road.side = 1;
        road.sideOffset = aside;
        road.standOnStones = true;

        var walk = FindAnywhere(WALK_GO);
        var line = walk != null ? walk.GetComponent<WordRiverPath>() : null;
        if (line != null && line.Valid)
        {
            road.climbWaypoints = WaypointsFor(line);
            log.Add($"climb now follows the authored stair: " +
                    $"{road.climbWaypoints.Count} waypoint(s) from " +
                    $"{line.points[0]} to {line.points[line.points.Length - 1]}.");
        }
        else
        {
            log.Add("NOTE: no authored walkway in the scene, so the climb still uses " +
                    "whatever waypoints the road already had. Run " +
                    "'8. Import Island Walkway From Blender'.");
        }

        road.Rebuild();
        log.Add($"route: ribbon off, back on the authored line, walking on TOP of " +
                $"the stones instead of through them — {road.StopCount} stop(s) " +
                $"over {road.Length:0.0} m.");
        log.Add(Measure(road));
    }

    /// <summary>
    /// Measure the two things this step is supposed to have changed, from the
    /// scene itself. Claiming "stones moved aside" in a log proves nothing — if a
    /// script silently no-ops, its log still reads like success. These are the
    /// numbers that say whether it actually took, so a "nothing changed" can be
    /// answered without guessing.
    /// </summary>
    static string Measure(RiversideRoad road)
    {
        var stone = road.Stone(0);
        if (stone == null) return "could not measure: the road has no stone 0.";

        Vector3 walk = road.PositionAt(road.StopDistance(0));
        float sideways = Vector3.ProjectOnPlane(stone.position - walk, Vector3.up).magnitude;

        var r = stone.GetComponentInChildren<Renderer>();
        float top = r != null ? r.bounds.max.y : stone.position.y;
        float clearance = walk.y - top;

        return $"MEASURED at word 1 — words sit {sideways:0.00} m to the side of the " +
               $"walk (want ≈ {STONE_ASIDE:0.00}; 0.00 means the offset did not take), " +
               $"and the reader's feet are {clearance:+0.00;-0.00} m relative to the " +
               $"stone top (want a small positive; negative means still walking " +
               $"through them).";
    }

    /// <summary>
    /// Put the reader back on the road. The road just moved — the ribbon is gone
    /// and the side offset went to zero — so wherever they were standing is beside
    /// where the walk now runs, and since the camera is pinned to them the whole
    /// shot inherits the error.
    /// </summary>
    static string StandReader(RiversideRoad road)
    {
        var walker = Object.FindAnyObjectByType<PathWalker>(FindObjectsInactive.Include);
        if (walker == null) return "no PathWalker in the scene — reader left alone.";

        Undo.RecordObject(walker.transform, "Rebuild World From Blender");
        float d = Mathf.Max(0f, road.StopDistance(0) - walker.approach);
        walker.transform.position = road.PositionAt(d);

        Vector3 heading = road.TangentAt(d);
        heading.y = 0f;
        if (heading.sqrMagnitude > 0.0001f)
            walker.transform.rotation =
                Quaternion.LookRotation(heading.normalized, Vector3.up);

        return $"reader re-stood at {walker.transform.position} " +
               $"(water sits at y ≈ 0.03, so feet above that means it landed right).";
    }

    /// <summary>
    /// Transforms for the road to pass through, one per authored walkway point.
    /// The road wants Transforms and the polyline is bare points, so they are
    /// parked under one child object rather than scattered through the scene.
    /// </summary>
    static List<Transform> WaypointsFor(WordRiverPath line)
    {
        var host = line.transform.Find(WALK_POINTS);
        if (host == null)
        {
            var go = new GameObject(WALK_POINTS);
            Undo.RegisterCreatedObjectUndo(go, "Rebuild World From Blender");
            go.transform.SetParent(line.transform, false);
            host = go.transform;
        }
        // rebuilt wholesale: a re-import can change how many points there are
        for (int i = host.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(host.GetChild(i).gameObject);

        var list = new List<Transform>();
        for (int i = 0; i < line.points.Length; i++)
        {
            var go = new GameObject($"Walk_{i:00}");
            Undo.RegisterCreatedObjectUndo(go, "Rebuild World From Blender");
            go.transform.SetParent(host, false);
            go.transform.position = line.points[i];
            list.Add(go.transform);
        }
        return list;
    }

    // ── going back ──────────────────────────────────────────────────────────

    [MenuItem("Tools/Great Library/Island/0b. Revert To The Old Island", true)]
    static bool RevertValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/0b. Revert To The Old Island")]
    public static void Revert()
    {
        int on = 0;
        foreach (var go in InstancesOf(OLD_FBX))
        {
            if (go.activeSelf) continue;
            Undo.RecordObject(go, "Revert To The Old Island");
            go.SetActive(true);
            on++;
        }

        var world = FindAnywhere(WORLD_GO);
        if (world != null)
        {
            Undo.RecordObject(world, "Revert To The Old Island");
            world.SetActive(false);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[World] Reverted: {on} old island instance(s) back on, " +
                  $"'{WORLD_GO}' switched off.\n" +
                  "  ! The river and walkway are still baked against the world at " +
                  "identity. Re-run importers 7 and 8 to solve them against the old " +
                  "island again — they will report a large residual, which is the " +
                  "problem this migration existed to remove.");
    }

    /// <summary>Find a GameObject by name in the open scene, switched off included.</summary>
    static GameObject FindAnywhere(string name)
    {
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (t.name == name) return t.gameObject;
        return null;
    }

    /// <summary>Every prefab-instance root in the open scene that came from this asset.</summary>
    static List<GameObject> InstancesOf(string assetPath)
    {
        var hits = new List<GameObject>();
        if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null) return hits;

        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                var go = t.gameObject;
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(go)) continue;
                var src = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (src == null) continue;
                if (AssetDatabase.GetAssetPath(src) == assetPath) hits.Add(go);
            }
        }
        return hits;
    }
}
