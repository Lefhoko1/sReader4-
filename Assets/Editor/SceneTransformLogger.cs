// ===========================================================================
//  SceneTransformLogger — dump every active GameObject's transform + bounds
// ===========================================================================
//  Tools > Great Library > Log Scene Transforms
//    Writes SceneDump.txt to the PROJECT ROOT (next to Assets/) with, for
//    every active object: hierarchy path, world position, euler rotation,
//    world (lossy) scale, and — when it has renderers — the WORLD-SPACE
//    bounds (centre + size). Bounds are what matter for "is it above the
//    water / how big is the island", not just the pivot.
//
//    Run it in Edit mode or Play mode. Safe & read-only.
// ===========================================================================
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransformLogger
{
    [MenuItem("Tools/Great Library/Log Scene Transforms")]
    public static void Dump()
    {
        var sb = new StringBuilder();
        var scene = EditorSceneManager.GetActiveScene();
        sb.AppendLine($"# Scene dump: {scene.name}   (Play mode: {Application.isPlaying})");
        sb.AppendLine("# format:  <path>  pos(x,y,z)  rot(x,y,z)  scale(x,y,z)  [bounds c(x,y,z) s(x,y,z)]");
        sb.AppendLine();

        int count = 0;
        foreach (var root in scene.GetRootGameObjects())
            count += Walk(root.transform, 0, sb);

        // whole-scene renderer extent (useful for sea level / island size)
        var all = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        if (all.Length > 0)
        {
            var b = all[0].bounds;
            foreach (var r in all) if (r.enabled) b.Encapsulate(r.bounds);
            sb.AppendLine();
            sb.AppendLine($"# SCENE RENDERER BOUNDS  centre {V(b.center)}  size {V(b.size)}  " +
                          $"minY {b.min.y.ToString("0.00", CultureInfo.InvariantCulture)}  maxY {b.max.y.ToString("0.00", CultureInfo.InvariantCulture)}");
        }

        var path = Path.Combine(Application.dataPath, "..", "SceneDump.txt");
        path = Path.GetFullPath(path);
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        Debug.Log($"[SceneDump] Wrote {count} active objects to {path}");
    }

    static int Walk(Transform t, int depth, StringBuilder sb)
    {
        if (!t.gameObject.activeInHierarchy) return 0;   // active objects only
        int count = 1;
        string indent = new string(' ', depth * 2);
        sb.Append(indent).Append(t.name);
        sb.Append("  pos").Append(V(t.position));
        sb.Append("  rot").Append(V(t.eulerAngles));
        sb.Append("  scale").Append(V(t.lossyScale));

        var rends = t.GetComponents<Renderer>();
        if (rends.Length > 0)
        {
            var b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            sb.Append("  bounds c").Append(V(b.center)).Append(" s").Append(V(b.size));
        }
        var cam = t.GetComponent<Camera>();
        if (cam != null)
            sb.Append($"  [Camera fov {cam.fieldOfView:0.#} ortho {cam.orthographic}]");
        sb.AppendLine();

        for (int i = 0; i < t.childCount; i++)
            count += Walk(t.GetChild(i), depth + 1, sb);
        return count;
    }

    static string V(Vector3 v) =>
        $"({v.x.ToString("0.00", CultureInfo.InvariantCulture)}, " +
        $"{v.y.ToString("0.00", CultureInfo.InvariantCulture)}, " +
        $"{v.z.ToString("0.00", CultureInfo.InvariantCulture)})";
}
