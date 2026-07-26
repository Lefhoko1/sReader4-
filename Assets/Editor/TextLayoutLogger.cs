// ===========================================================================
//  TextLayoutLogger — dump the book/board text layout WHILE PLAYING
// ===========================================================================
//  Tools > Great Library > Log Text Layout          (works in Play mode)
//
//  Press Play, let the text build, then run this. It writes TextDump.txt to
//  the project root with everything needed to size text correctly:
//
//    • the camera (position, rotation, fov)
//    • the book/board root: transform, world bounds, its right/up/forward axes
//    • every SOCKET_* : world + local position
//    • every TextMeshPro: the text, the RESOLVED font size after auto-sizing,
//      rect size, world transform, renderer bounds (the real on-screen size),
//      whether its renderer is enabled, and — critically — whether it FACES
//      the camera (TMP is single-sided, so a back-facing label is invisible).
// ===========================================================================
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class TextLayoutLogger
{
    [MenuItem("Tools/Great Library/Log Text Layout")]
    public static void Dump()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Text layout dump   (Play mode: {Application.isPlaying})");

        var cam = Camera.main;
        if (cam != null)
            sb.AppendLine($"# Camera  pos{V(cam.transform.position)}  rot{V(cam.transform.eulerAngles)}  " +
                          $"fov {cam.fieldOfView:0.#}  ortho {cam.orthographic}");
        sb.AppendLine();

        foreach (var rootName in new[] { "StoryBook", "SM_SkyBoard", "SM_Book_Hero_Open" })
        {
            var go = GameObject.Find(rootName);
            if (go == null) { sb.AppendLine($"## {rootName}: NOT IN SCENE"); sb.AppendLine(); continue; }
            DumpRoot(go, cam, sb);
        }

        var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TextDump.txt"));
        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
        Debug.Log($"[TextDump] Wrote {path}");
    }

    static void DumpRoot(GameObject go, Camera cam, StringBuilder sb)
    {
        var t = go.transform;
        sb.AppendLine($"## {go.name}");
        sb.AppendLine($"   transform  pos{V(t.position)}  rot{V(t.eulerAngles)}  lossyScale{V(t.lossyScale)}");
        sb.AppendLine($"   axes       right{V(t.right)}  up{V(t.up)}  forward{V(t.forward)}");

        // world bounds of the model itself (excluding the text we added)
        var rends = go.GetComponentsInChildren<Renderer>(true);
        bool has = false; Bounds b = default;
        foreach (var r in rends)
        {
            if (r.GetComponent<TextMeshPro>() != null) continue;   // skip the text we added
            if (!has) { b = r.bounds; has = true; } else b.Encapsulate(r.bounds);
        }
        if (has) sb.AppendLine($"   modelBounds centre{V(b.center)}  size{V(b.size)}");

        // the mesh's LOCAL bounds tell us the true page dimensions before rotation
        var mf = go.GetComponentInChildren<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            var lb = mf.sharedMesh.bounds;
            sb.AppendLine($"   meshLocal  centre{V(lb.center)}  size{V(lb.size)}  " +
                          $"(on '{mf.name}', lossyScale{V(mf.transform.lossyScale)})");
        }

        sb.AppendLine("   -- sockets --");
        foreach (var c in go.GetComponentsInChildren<Transform>(true))
            if (c.name.StartsWith("SOCKET"))
                sb.AppendLine($"   {c.name,-24} world{V(c.position)}  local{V(c.localPosition)}  lossyScale{V(c.lossyScale)}");

        sb.AppendLine("   -- text --");
        var texts = go.GetComponentsInChildren<TextMeshPro>(true);
        if (texts.Length == 0) sb.AppendLine("   (none — text has not been built)");
        foreach (var tm in texts)
        {
            var r = tm.GetComponent<Renderer>();
            string facing = "n/a";
            if (cam != null)
            {
                // TMP is visible when its forward points away from the camera
                float d = Vector3.Dot(tm.transform.forward, tm.transform.position - cam.transform.position);
                facing = d > 0 ? "FACING-OK" : "BACK-FACING (invisible!)";
            }
            sb.AppendLine($"   [{tm.gameObject.name}] \"{Trim(tm.text)}\"");
            sb.AppendLine($"      pos{V(tm.transform.position)}  rot{V(tm.transform.eulerAngles)}  lossyScale{V(tm.transform.lossyScale)}");
            sb.AppendLine($"      fontSize {tm.fontSize:0.###} (auto {tm.enableAutoSizing}, range {tm.fontSizeMin:0.##}-{tm.fontSizeMax:0.##})  " +
                          $"rect{V2(tm.rectTransform.sizeDelta)}");
            sb.AppendLine($"      rendererEnabled {(r != null && r.enabled)}  " +
                          $"renderedSize{(r != null ? V(r.bounds.size) : "n/a")}  {facing}");
        }
        sb.AppendLine();
    }

    static string Trim(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = s.Replace("\n", " ");
        return s.Length <= 60 ? s : s.Substring(0, 57) + "...";
    }

    static string V(Vector3 v) =>
        $"({v.x.ToString("0.###", CultureInfo.InvariantCulture)}, " +
        $"{v.y.ToString("0.###", CultureInfo.InvariantCulture)}, " +
        $"{v.z.ToString("0.###", CultureInfo.InvariantCulture)})";

    static string V2(Vector2 v) =>
        $"({v.x.ToString("0.###", CultureInfo.InvariantCulture)}, " +
        $"{v.y.ToString("0.###", CultureInfo.InvariantCulture)})";
}
