using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Import diagnostics for the Reading House scene: model root transforms and
    /// every renderer whose bounds sit in front of the gameplay / overview
    /// cameras, so Blender→Unity transform problems can be located by name.
    /// Read-only: the scene is closed without saving.
    /// </summary>
    public static class ReadingHouseDiagnostics
    {
        [MenuItem("Tools/sReader/Reading House/Write Import Diagnostics")]
        public static void Run()
        {
            var sb = new StringBuilder();
            var (scene, discard) = ReadingHouseSceneBuilder.OpenForCheck();
            try
            {
                foreach (var root in scene.GetRootGameObjects())
                    sb.AppendLine($"ROOT {root.name} pos {root.transform.position} rot {root.transform.eulerAngles} scale {root.transform.lossyScale}");

                var rig = scene.GetRootGameObjects().Select(r => r.GetComponentInChildren<ReadingHouseCameraRig>(true)).First(r => r != null);
                foreach (var pose in rig.poses)
                {
                    sb.AppendLine($"\nPOSE {pose.view} pos {pose.position.position} aim {pose.aim.position} fov {pose.referenceVerticalFov}");
                    var origin = pose.position.position;
                    var fwd = (pose.aim.position - origin).normalized;
                    var near = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Renderer>(true))
                        .Select(r => (r, d: Vector3.Dot(r.bounds.center - origin, fwd)))
                        .Where(x => x.d > -0.5f && x.d < 3.2f && Vector3.Angle(fwd, x.r.bounds.center - origin) < 40f)
                        .OrderBy(x => x.d).Take(25);
                    foreach (var (r, d) in near)
                        sb.AppendLine($"  {d:0.00}m  {Path(r.transform)}  centre {r.bounds.center}  size {r.bounds.size}");
                }

                // Everything in front of the tray in the gameplay view, with the student at his gameplay spot.
                var presenter = scene.GetRootGameObjects().Select(r => r.GetComponentInChildren<ReadingHousePresenter>(true)).First(p => p != null);
                presenter.studentMover.ToGameplay(immediate: true);
                var cam = rig.viewCamera;
                cam.aspect = ReadingHouseCameraRig.ReferenceAspect;
                rig.GoTo(ReadingHouseView.Gameplay, immediate: true);
                float trayDist = Vector3.Distance(cam.transform.position, presenter.tray.transform.position);
                sb.AppendLine($"\nIN FRONT OF THE TRAY (gameplay view, tray at {trayDist:0.00} m):");
                foreach (var r in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Renderer>(false)))
                {
                    var vp = cam.WorldToViewportPoint(r.bounds.center);
                    if (vp.z <= 0 || vp.z > trayDist - 0.3f || vp.x < -0.1f || vp.x > 1.1f || vp.y < -0.1f || vp.y > 1.1f) continue;
                    sb.AppendLine($"  {vp.z:0.00}m  vp({vp.x:0.00},{vp.y:0.00})  {Path(r.transform)}  size {r.bounds.size}");
                }
                presenter.LoadTestPassage();
                presenter.ShowSentence(1);
                sb.AppendLine("\nSTONES (sentence 2):");
                foreach (var s in presenter.tray.Active.Take(4))
                    sb.AppendLine($"  {s.Word}: pos {s.transform.position} rot {s.transform.eulerAngles} scale {s.transform.localScale.x:0.00} " +
                                  $"colliderSize {s.tapCollider.size} bounds {s.tapCollider.bounds.center} {s.tapCollider.bounds.size} centreScale {s.centre.localScale} capL {s.capLeft.localPosition} label {s.label.transform.position}");
                sb.AppendLine("\nOPEN SCENES: " + string.Join(", ", Enumerable.Range(0, SceneManager.sceneCount).Select(i => SceneManager.GetSceneAt(i).path)));
                sb.AppendLine("\nOBJECTS ON THE RIGHT WALL SIDE (Unity x < -3.2):");
                foreach (var r in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Renderer>(false)).Where(r => r.bounds.center.x < -3.2f && r.bounds.size.y > 0.8f))
                    sb.AppendLine($"  {Path(r.transform)} centre {r.bounds.center} size {r.bounds.size}");

                foreach (var name in new[] { "SR_Bookshelf_Module_West_1", "SR_Bookshelf_Module_East_2", "SR_ENCOURAGER_BODY_white_OPT", "SR_CHARACTER_ENCOURAGER_OPT", "SR_DB_Fascia", "SR_DISPLAY_MAIN" })
                {
                    var t = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).FirstOrDefault(x => x.name == name);
                    if (t == null) { sb.AppendLine($"MISSING {name}"); continue; }
                    var rr = t.GetComponent<Renderer>();
                    sb.AppendLine($"OBJ {Path(t)} pos {t.position} rot {t.eulerAngles} lossy {t.lossyScale} bounds {(rr ? rr.bounds.center.ToString() : "-")} {(rr ? rr.bounds.size.ToString() : "")}");
                }
            }
            finally { discard(); }
            var file = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "verification", "IMPORT_DIAGNOSTICS.txt"));
            File.WriteAllText(file, sb.ToString());
        }

        static string Path(Transform t) => t.parent == null ? t.name : Path(t.parent) + "/" + t.name;
    }
}
