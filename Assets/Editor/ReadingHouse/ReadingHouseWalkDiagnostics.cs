using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>Why does the student fall? Reports what is under his start spot and how his controller is set up. Read-only.</summary>
    public static class ReadingHouseWalkDiagnostics
    {
        public static void Run()
        {
            var sb = new StringBuilder();
            var (scene, discard) = ReadingHouseSceneBuilder.OpenForCheck();
            try
            {
                var roots = scene.GetRootGameObjects();
                var director = roots.Select(r => r.GetComponentInChildren<ReadingHouseExploreDirector>(true)).First(d => d != null);
                var student = director.walker.transform;
                var cc = student.GetComponent<CharacterController>();
                Physics.SyncTransforms();
                sb.AppendLine($"student root '{student.name}' pos {student.position} rot {student.eulerAngles} lossyScale {student.lossyScale} parent {(student.parent ? student.parent.name : "-")}");
                sb.AppendLine($"cc center {cc.center} height {cc.height} radius {cc.radius} enabled {cc.enabled} bounds {cc.bounds.center} {cc.bounds.size}");
                var start = director.studentStart.position;
                sb.AppendLine($"start {start}");
                foreach (var p in new[] { start, director.door.transform.position + Vector3.up * 0.2f, student.position })
                {
                    var hits = Physics.RaycastAll(p + Vector3.up * 5f, Vector3.down, 30f).OrderBy(h => h.distance);
                    sb.AppendLine($"ray down at {p}:");
                    foreach (var h in hits) sb.AppendLine($"   hit {h.collider.name} ({h.collider.GetType().Name}) at y {h.point.y:0.000} normal {h.normal}");
                }
                int mc = roots.SelectMany(r => r.GetComponentsInChildren<MeshCollider>(true)).Count();
                sb.AppendLine($"mesh colliders in scene: {mc}");
                foreach (var name in new[] { "SR_Foundation", "SR_Wood_Floor", "SR_Garden_Plinth", "SR_Garden_Moss" })
                {
                    var t = roots.SelectMany(r => r.GetComponentsInChildren<Transform>(true)).FirstOrDefault(x => x.name == name);
                    if (t == null) { sb.AppendLine($"{name}: missing"); continue; }
                    var c = t.GetComponent<MeshCollider>(); var mf = t.GetComponent<MeshFilter>();
                    sb.AppendLine($"{name}: collider {(c ? "yes" : "NO")} enabled {(c && c.enabled)} mesh {(c && c.sharedMesh ? c.sharedMesh.name + " v" + c.sharedMesh.vertexCount : "none")} readable {(mf && mf.sharedMesh ? mf.sharedMesh.isReadable.ToString() : "-")} bounds {(c ? c.bounds.ToString() : "-")} rot {t.eulerAngles} scale {t.lossyScale}");
                }
            }
            finally { discard(); }
            File.WriteAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "verification", "WALK_DIAGNOSTICS.txt")), sb.ToString());
        }
    }
}
