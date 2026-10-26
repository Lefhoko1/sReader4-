using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>Lists the dashboard's parts (and the bookshelves) with their bounds and roles, for the chalkboard rework. Read-only.</summary>
    public static class DashboardProbe
    {
        public static void Run()
        {
            var scene = SceneManager.GetActiveScene();
            var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
            var sb = new StringBuilder();
            var root = all.FirstOrDefault(t => t.name == "SR_DASHBOARD_ROOT");
            if (root != null)
            {
                sb.AppendLine($"DASHBOARD root {root.position} parent {root.parent?.name}");
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    var r = t.GetComponent<Renderer>();
                    string extra = (t.GetComponent<DashboardControlTarget>() != null ? " [CONTROL " + t.GetComponent<DashboardControlTarget>().control + "]" : "")
                                 + (t.GetComponent<TMPro.TMP_Text>() != null ? " [TMP '" + t.GetComponent<TMPro.TMP_Text>().text.Replace("\n", " ") + "']" : "")
                                 + (t.GetComponent<Collider>() != null ? " [col]" : "")
                                 + (t.GetComponent<WorldUIScreen>() != null ? " [SCREEN]" : "");
                    int depth = 0; for (var p = t; p != root && p != null; p = p.parent) depth++;
                    sb.AppendLine($"{new string(' ', depth * 2)}{t.name} active={t.gameObject.activeSelf}" +
                                  (r != null ? $" centre {r.bounds.center:F2} size {r.bounds.size:F2} mat {r.sharedMaterial?.name}" : "") + extra);
                }
            }
            var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).FirstOrDefault(p => p != null);
            if (presenter != null)
            {
                var d = presenter.dashboard;
                foreach (var l in new TMPro.TextMeshPro[] { d.pointsLabel, d.pointsValue, d.menuLabel, d.assignmentTitle })
                    if (l != null) sb.AppendLine($"LABEL {l.name} '{l.text}' pos {l.transform.position:F2} rot {l.transform.eulerAngles:F0} parent {l.transform.parent?.name} size {l.fontSize} rect {l.rectTransform.sizeDelta} active {l.gameObject.activeInHierarchy}");
            }
            foreach (var r in all.Select(t => t.GetComponent<Renderer>()).Where(r => r != null && r.enabled && r.gameObject.activeInHierarchy))
            {
                var b = r.bounds;
                if (b.center.y > 3.55f && b.center.y < 3.95f && Mathf.Abs(b.center.x) < 1.2f && b.center.z < -1.6f && b.center.z > -2.6f)
                    sb.AppendLine($"NEAR-PLATE {r.name} ({r.GetType().Name}) centre {b.center:F2} size {b.size:F2} parent {r.transform.parent?.name} root {r.transform.root.name}");
            }
            foreach (var t in all.Where(t => t.GetComponent<TMPro.TMP_Text>() != null && (t.GetComponent<TMPro.TMP_Text>().text ?? "").Contains("Reader")))
                sb.AppendLine($"READER-TEXT {t.name} '{t.GetComponent<TMPro.TMP_Text>().text}' pos {t.position:F2} root {t.root.name} parent {t.parent?.name} active {t.gameObject.activeInHierarchy}");
            foreach (var t in all.Where(t => t.GetComponent<TMPro.TMP_Text>() != null && t.root.name == "Dashboard"))
                sb.AppendLine($"TMP {t.name} '{t.GetComponent<TMPro.TMP_Text>().text}' pos {t.position:F2}");
            var director = all.Select(t => t.GetComponent<ReadingHouseExploreDirector>()).FirstOrDefault(d => d != null);
            var postR = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Post");
            var lib = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Sign_Library");
            var hdr = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Sign_Header");
            var lan = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Lantern_Glass");
            if (director != null && postR != null && lib != null)
            {
                var cam = director.exteriorCameraPos.position;
                var toCam = Vector3.ProjectOnPlane(cam - postR.bounds.center, Vector3.up).normalized;
                var right = Vector3.Cross(Vector3.up, -toCam);
                sb.AppendLine($"SIGN post {postR.bounds.center:F2} library {lib.bounds.center:F2} header {hdr?.bounds.center:F2} lantern {lan?.bounds.center:F2} cam {cam:F2}");
                sb.AppendLine($"SIGN library vs post: towards camera {Vector3.Dot(lib.bounds.center - postR.bounds.center, toCam):F2}, screen-right {Vector3.Dot(lib.bounds.center - postR.bounds.center, right):F2}; lantern screen-right {Vector3.Dot(lan.bounds.center - postR.bounds.center, right):F2}");
            }
            var seat = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Seat_0");
            var back = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == "Back_1");
            if (director != null && seat != null && back != null)
            {
                var cam = director.exteriorCameraPos.position;
                var toCam = Vector3.ProjectOnPlane(cam - back.bounds.center, Vector3.up).normalized;
                sb.AppendLine($"BENCH seat-front vs back towards camera {Vector3.Dot(seat.bounds.center - back.bounds.center, toCam):F2} (positive = we see its front)");
            }
            foreach (var t in all.Where(t => t.name.StartsWith("Candy_") || t.name.StartsWith("JellyBean_")))
            {
                var r = t.GetComponent<Renderer>();
                var mf = t.GetComponent<MeshFilter>();
                sb.AppendLine($"CANDY {t.name} mat {r?.sharedMaterial?.name} centre {r?.bounds.center:F2} scale {t.lossyScale:F2} verts {mf?.sharedMesh?.vertexCount} normals {mf?.sharedMesh?.normals?.Length}");
            }
            foreach (var t in all.Where(t => t.name.ToLower().Contains("shelf") || t.name.ToLower().Contains("book")).Take(40))
            {
                var r = t.GetComponent<Renderer>();
                sb.AppendLine($"SHELF {t.name} parent {t.parent?.name}" + (r != null ? $" centre {r.bounds.center:F2} size {r.bounds.size:F2}" : ""));
            }
            System.IO.File.WriteAllText(System.IO.Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "dashboard_probe.txt"), sb.ToString());
            Debug.Log("[DashboardProbe] written");
        }
    }
}
