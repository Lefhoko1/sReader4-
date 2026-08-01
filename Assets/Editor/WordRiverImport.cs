// ===========================================================================
//  WordRiverImport — bring the river drawn in Blender into the Unity scene.
// ===========================================================================
//  Tools > Great Library > Island > 7. Import River Path From Blender
//  Tools > Great Library > Island > 8. Import Island Walkway From Blender
//
//  Reads Assets/Art/Models_Island/WordRiver_Path.json (written by the Blender
//  side) and bakes its polyline into a WordRiverPath in the open scene, then
//  points the WordPathBuilder at it and rebuilds.
//
//  TWO AUTHORED LINES, ONE PIPELINE. The river (PATH_WordRiver) is where the word
//  STONES sit; the walkway (SM_Island_Path) is where the READER walks once ashore.
//  Same schema, same landmarks, same axis solve — see Bake().
//
//  THE AXIS PROBLEM, AND WHY NOTHING HERE IS HARD-CODED
//  Blender is Z-up, Unity is Y-up, and exactly which conversion an FBX went
//  through depends on export flags and importer settings that are easy to change
//  by accident. Guessing it wrong puts the river through the island, or behind
//  the camera, in a way that looks like a bug in the path code.
//
//  So the JSON also carries the Blender positions of objects that exist BY NAME
//  in the scene (SM_Dock, SM_Boat, ...). This tries every plausible conversion,
//  and for each one solves the leftover translation, then keeps whichever lines
//  those landmarks up best. The residual is logged: if it is not near zero, the
//  island in the scene is not the island the river was drawn against, and the
//  log says so rather than leaving you to wonder.
// ===========================================================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WordRiverImport
{
    const string JSON = "Assets/Art/Models_Island/WordRiver_Path.json";
    const string RIVER_GO = "WordRiver_Blender";

    const string WALK_JSON = "Assets/Art/Models_Island/IslandWalk_Path.json";
    const string WALK_GO = "IslandWalk_Blender";

    // ---- the JSON, in a shape JsonUtility can actually read -----------------
    [System.Serializable] class BVec { public float x, y, z; }
    [System.Serializable] class RiverFile
    {
        public string note, source;
        public float waterZ;
        public BVec[] points;
        public string[] refNames;
        public BVec[] refPos;

        // walkway files only
        public float width;
        public int steps;
    }

    /// <summary>One candidate Blender→Unity axis conversion.</summary>
    struct Axes
    {
        public string name;
        public System.Func<BVec, Vector3> map;
    }

    static readonly Axes[] CANDIDATES =
    {
        new Axes { name = "(x, z, y)",   map = v => new Vector3( v.x, v.z,  v.y) },
        new Axes { name = "(x, z, -y)",  map = v => new Vector3( v.x, v.z, -v.y) },
        new Axes { name = "(-x, z, y)",  map = v => new Vector3(-v.x, v.z,  v.y) },
        new Axes { name = "(-x, z, -y)", map = v => new Vector3(-v.x, v.z, -v.y) },
    };

    [MenuItem("Tools/Great Library/Island/7. Import River Path From Blender", true)]
    static bool ImportValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/7. Import River Path From Blender")]
    public static void Import()
    {
        var river = Bake(JSON, RIVER_GO, "Import River Path",
                         out var file, out var axes, out Vector3 offset,
                         out float residual, out int matched);
        if (river == null) return;
        var pts = river.points;

        // ---------- hand it to the builder -----------------------------------
        var builder = Object.FindAnyObjectByType<WordPathBuilder>();
        if (builder != null)
        {
            Undo.RecordObject(builder, "Import River Path");
            builder.river = river;
            builder.usePool = true;
            EditorUtility.SetDirty(builder);
            builder.Build();
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = river.gameObject;

        float len = river.Length;
        Debug.Log(
            $"[River] Imported {pts.Length} points, {len:0.0} m of river.\n" +
            $"  • axes {axes.name}, offset {offset}, matched {matched} landmark(s), " +
            $"residual {residual:0.000} m {(residual < 0.05f ? "— exact" : residual < 0.5f ? "— close enough" : "— SUSPECT, see below")}\n" +
            $"  • ends: {pts[0]} (open water) → {pts[pts.Length - 1]} (the shore)\n" +
            (builder != null
                ? $"  • {builder.name} now follows it, pooled at {builder.poolSlots} slots.\n"
                : "  • no WordPathBuilder in the scene — assign the river by hand.\n") +
            (residual >= 0.5f
                ? "  ! The landmarks do not line up. The island in this scene is probably " +
                  "not the one the river was drawn against — re-export the island FBX, or " +
                  "re-draw the river against the current one.\n"
                : ""));
    }

    // =======================================================================
    //  The island walkway — SM_Island_Path, the stair from the dock to the door
    // =======================================================================
    //  The river above is the line the WORD STONES sit on. This is the line the
    //  READER walks once they are ashore, and it is authored art, not something
    //  to invent: it is the centre line of the six-step stair modelled in the
    //  island. Imported as its own polyline and deliberately NOT handed to the
    //  WordPathBuilder — put the words on this and the sentence climbs the steps.

    [MenuItem("Tools/Great Library/Island/8. Import Island Walkway From Blender", true)]
    static bool ImportWalkValidate() => !Application.isPlaying;

    [MenuItem("Tools/Great Library/Island/8. Import Island Walkway From Blender")]
    public static void ImportWalk()
    {
        var walk = Bake(WALK_JSON, WALK_GO, "Import Island Walkway",
                        out var file, out var axes, out Vector3 offset,
                        out float residual, out int matched);
        if (walk == null) return;

        walk.gizmoColor = new Color(0.55f, 0.85f, 1f, 1f);
        EditorUtility.SetDirty(walk);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = walk.gameObject;

        Debug.Log(
            $"[Walk] Imported the authored walkway: {file.steps} steps, " +
            $"{walk.Length:0.0} m, {file.width:0.00} m wide.\n" +
            $"  • axes {axes.name}, offset {offset}, matched {matched} landmark(s), " +
            $"residual {residual:0.000} m " +
            $"{(residual < 0.05f ? "— exact" : residual < 0.5f ? "— close enough" : "— SUSPECT")}\n" +
            $"  • ends: {walk.points[0]} (the dock) → " +
            $"{walk.points[walk.points.Length - 1]} (the entrance terrace)\n" +
            $"  • nothing follows it yet — it is scene data. It is NOT given to the " +
            $"WordPathBuilder on purpose: the words belong on the river, not the stairs.");
    }

    /// <summary>
    /// Read a Blender polyline file, solve its axis conversion against the island
    /// in the open scene, and bake it into a named WordRiverPath. Shared by the
    /// river and the walkway — same schema, same landmarks, same solve.
    /// </summary>
    static WordRiverPath Bake(string json, string goName, string undo,
                              out RiverFile file, out Axes axes, out Vector3 offset,
                              out float residual, out int matched)
    {
        axes = CANDIDATES[0]; offset = Vector3.zero; residual = 0f; matched = 0;

        file = Load(json);
        if (file == null) return null;
        if (file.points == null || file.points.Length < 2)
        {
            Debug.LogError($"[River] {json} has no polyline. Re-run the Blender export.");
            return null;
        }
        if (!Solve(file, out axes, out offset, out residual, out matched)) return null;

        // Locals: `map` and `off` are copies because an out parameter cannot be
        // captured by a lambda (CS1628).
        var map = axes.map;
        var off = offset;
        var pts = file.points.Select(p => map(p) + off).ToArray();

        var go = GameObject.Find(goName);
        if (go == null)
        {
            go = new GameObject(goName);
            Undo.RegisterCreatedObjectUndo(go, undo);
        }
        var line = go.GetComponent<WordRiverPath>();
        if (line == null) line = Undo.AddComponent<WordRiverPath>(go);

        Undo.RecordObject(line, undo);
        line.points = pts;
        line.Invalidate();
        EditorUtility.SetDirty(line);
        return line;
    }

    static RiverFile Load(string json)
    {
        string abs = Path.GetFullPath(json);
        if (!File.Exists(abs))
        {
            Debug.LogError($"[River] {json} not found. Draw the path in Blender and " +
                           "run the export there first.");
            return null;
        }
        try
        {
            return JsonUtility.FromJson<RiverFile>(File.ReadAllText(abs));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[River] Could not read {JSON}: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Work out which axis conversion (and what leftover translation) takes the
    /// Blender landmark positions onto the ones in the open scene. Solving the
    /// translation as well means the island prefab does not have to sit at the
    /// origin for this to work.
    /// </summary>
    static bool Solve(RiverFile file, out Axes best, out Vector3 offset,
                      out float residual, out int matched)
    {
        best = CANDIDATES[0]; offset = Vector3.zero; residual = 0f; matched = 0;

        var pairs = new List<(BVec blender, Vector3 unity)>();
        for (int i = 0; file.refNames != null && i < file.refNames.Length; i++)
        {
            if (file.refPos == null || i >= file.refPos.Length) break;
            var t = FindInScene(file.refNames[i]);
            if (t != null) pairs.Add((file.refPos[i], t.position));
        }
        matched = pairs.Count;

        if (pairs.Count == 0)
        {
            Debug.LogError(
                "[River] None of the landmarks (" +
                string.Join(", ", file.refNames ?? new string[0]) +
                ") are in the open scene, so the Blender→Unity conversion cannot be " +
                "solved. Open SC_IslandVista with the island prefab in it and try again.");
            return false;
        }

        float bestErr = float.MaxValue;
        foreach (var cand in CANDIDATES)
        {
            // the translation that best lines this candidate up = the mean gap
            Vector3 sum = Vector3.zero;
            foreach (var (b, u) in pairs) sum += u - cand.map(b);
            Vector3 off = sum / pairs.Count;

            float err = pairs.Sum(p => Vector3.Distance(cand.map(p.blender) + off, p.unity))
                        / pairs.Count;
            if (err < bestErr) { bestErr = err; best = cand; offset = off; }
        }
        residual = bestErr;

        // One landmark can be fitted perfectly by ANY candidate — the offset just
        // absorbs the error — so the answer is a guess rather than a solve.
        if (pairs.Count == 1)
            Debug.LogWarning("[River] Only one landmark matched, so the axis conversion " +
                             "is assumed rather than solved. Check the river sits on the " +
                             "water before trusting it.");
        return true;
    }

    /// <summary>
    /// Find a landmark by name in the open scene. ACTIVE objects win: during the
    /// world migration the old island is left in the scene but switched off, and
    /// solving the axes against a retired copy would silently place the river
    /// against a world nobody can see. Inactive is only a fallback.
    /// </summary>
    static Transform FindInScene(string name)
    {
        Transform inactive = null;
        foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name != name) continue;
                if (t.gameObject.activeInHierarchy) return t;
                if (inactive == null) inactive = t;
            }
        }
        return inactive;
    }
}
