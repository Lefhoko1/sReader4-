using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Phase-4 checks for the Reading House integration scene, run in the editor
    /// without entering Play mode. Opens the scene additively, drives it through
    /// the real presenter (existing TrailBuilder / TrekSession pipeline), and
    /// writes renders + a report to GeneratedAssets/ReadingHouseUnity/verification.
    /// The scene is closed WITHOUT saving, so the checks leave nothing behind.
    /// </summary>
    public static class ReadingHouseVerification
    {
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "verification"));
        const int Width = 1520, Height = 720;

        [MenuItem("Tools/sReader/Reading House/Run Verification")]
        public static void RunAll()
        {
            Directory.CreateDirectory(Out);
            var report = new StringBuilder();
            var (scene, discard) = ReadingHouseSceneBuilder.OpenForCheck();
            var tempColliders = new List<Collider>();
            var rt = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 1 };
            try
            {
                var presenter = Get<ReadingHousePresenter>(scene);
                var rig = presenter.cameraRig;
                var cam = rig.viewCamera;
                cam.targetTexture = rt;
                var dash = presenter.dashboard;
                void View(ReadingHouseView v)
                {
                    dash.SetDialogueLayout(v != ReadingHouseView.Overview);
                    rig.GoTo(v, immediate: true);
                }
                dash.SetControlLabels(presenter.pointsLabel, presenter.menuLabel, presenter.playWithOthersLabel);
                dash.SetDialogue(presenter.encouragerLine, presenter.zombieLine);
                dash.SetIllustration(presenter.sampleIllustration);

                // Temporary mesh colliders on the characters so rays can detect them.
                var charactersRoot = presenter.studentMover.student.root;
                foreach (var mf in charactersRoot.GetComponentsInChildren<MeshFilter>(true))
                {
                    var mc = mf.gameObject.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    tempColliders.Add(mc);
                }

                report.AppendLine("# Reading House — Unity integration verification");
                report.AppendLine($"Generated {DateTime.Now:yyyy-MM-dd HH:mm}. Editor checks only; device numbers come from the APK overlay.\n");

                // ── stones: 5 / 11 / 20 words through the real pipeline ──
                presenter.LoadTestPassage();
                report.AppendLine("## Stone generation (test passage via TrailBuilder.BuildFromPlainText)\n");
                report.AppendLine("| Sentence | Words | Rows | Scale | Labels match | Labels fit | Overlaps | Gameplay blocked | Tray-focus blocked |");
                report.AppendLine("|---|---|---|---|---|---|---|---|---|");
                for (int i = 0; i < presenter.Trail.Sentences.Count && i < 3; i++)
                {
                    presenter.ShowSentence(i);
                    Physics.SyncTransforms();   // edit mode: collider bounds only follow moved stones after a sync
                    var stones = presenter.tray.Active.ToList();
                    var layout = presenter.tray.LastLayout;
                    bool labelsMatch = stones.All(s => s.label.text == presenter.Words[s.WordIndex].text);
                    bool labelsFit = stones.All(s => s.label.GetPreferredValues(s.Word).x * s.label.transform.localScale.x <= s.tapCollider.size.x + 1e-3f);
                    int overlaps = CountOverlaps(stones);

                    presenter.studentMover.ToGameplay(immediate: true);
                    View(ReadingHouseView.Gameplay);
                    Physics.SyncTransforms();
                    int blockedGame = CountBlocked(cam, stones.Select(s => s.tapCollider as Collider), charactersRoot);
                    Capture(cam, rt, $"gameplay_{stones.Count:00}_words.png");
                    View(ReadingHouseView.TrayFocus);
                    int blockedTray = CountBlocked(cam, stones.Select(s => s.tapCollider as Collider), charactersRoot);
                    Capture(cam, rt, $"trayfocus_{stones.Count:00}_words.png");

                    report.AppendLine($"| {i + 1} | {stones.Count} | {layout.RowCount} | {layout.Scale:0.00} | {Yes(labelsMatch)} | {Yes(labelsFit)} | {overlaps} | {blockedGame} | {blockedTray} |");
                }

                // ── controls ──
                report.AppendLine("\n## Dashboard controls in the gameplay view\n");
                presenter.ShowSentence(1);
                presenter.studentMover.ToGameplay(immediate: true);
                View(ReadingHouseView.Gameplay);
                Physics.SyncTransforms();
                foreach (var target in Object.FindObjectsByType<DashboardControlTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (target.gameObject.scene != scene) continue;
                    var col = target.GetComponent<Collider>();
                    var vp = cam.WorldToViewportPoint(col.bounds.center);
                    bool onScreen = vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
                    bool hits = Physics.Raycast(cam.ScreenPointToRay(cam.WorldToScreenPoint(col.bounds.center)), out var hit, 50f) && hit.collider == col;
                    report.AppendLine($"- {target.control}: on screen {Yes(onScreen)}, a tap at its centre reaches it {Yes(hits)}");
                }

                // ── overview + sample book ──
                presenter.studentMover.ToOverview(immediate: true);
                View(ReadingHouseView.Overview);
                Capture(cam, rt, "overview_test_passage.png");
                presenter.LoadFromBestSource();
                presenter.studentMover.ToGameplay(immediate: true);
                View(ReadingHouseView.Gameplay);
                Capture(cam, rt, "gameplay_sample_book.png");
                report.AppendLine($"\n## Sample book (placeholder mode)\n\nTitle \"{presenter.Trail.Title}\", {presenter.Trail.Sentences.Count} sentences, {presenter.Trail.Gates.Count} key words (gold stones).");

                // ── budget ──
                report.AppendLine("\n## Scene budget (editor counts)\n");
                AppendStats(report, scene);
                report.AppendLine("\nFrame rate is not measured in the editor. Run the development APK on the Huawei Y5; the overlay shows FPS, draw calls, SetPass calls, triangles and memory from the device.");
            }
            finally
            {
                foreach (var c in tempColliders) if (c != null) Object.DestroyImmediate(c);
                foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) if (cam.targetTexture == rt) cam.targetTexture = null;
                rt.Release();
                Object.DestroyImmediate(rt);
                discard();   // verification changes nothing
            }
            File.WriteAllText(Path.Combine(Out, "VERIFICATION_REPORT.md"), report.ToString());
            Debug.Log($"[ReadingHouse] Verification written to {Out}");
        }

        static int CountOverlaps(List<WordStoneView> stones)
        {
            int n = 0;
            for (int a = 0; a < stones.Count; a++)
                for (int b = a + 1; b < stones.Count; b++)
                {
                    var ba = stones[a].tapCollider.bounds; var bb = stones[b].tapCollider.bounds;
                    ba.Expand(-0.01f); bb.Expand(-0.01f);
                    if (ba.Intersects(bb)) n++;
                }
            return n;
        }

        /// <summary>Rays from the camera to each target's centre and four inset corners; counts targets a character hides.</summary>
        static int CountBlocked(Camera cam, IEnumerable<Collider> targets, Transform characters)
        {
            int blocked = 0;
            foreach (var col in targets)
            {
                var b = col.bounds;
                var points = new[]
                {
                    b.center,
                    b.center + new Vector3(b.extents.x, b.extents.y, 0) * 0.7f, b.center + new Vector3(-b.extents.x, b.extents.y, 0) * 0.7f,
                    b.center + new Vector3(b.extents.x, -b.extents.y, 0) * 0.7f, b.center + new Vector3(-b.extents.x, -b.extents.y, 0) * 0.7f,
                };
                foreach (var p in points)
                {
                    var dir = p - cam.transform.position;
                    if (Physics.Raycast(cam.transform.position, dir.normalized, out var hit, dir.magnitude) && hit.collider.transform.IsChildOf(characters))
                    { blocked++; break; }
                }
            }
            return blocked;
        }

        static void Capture(Camera cam, RenderTexture rt, string file)
        {
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
            if (RenderPipeline.SupportsRenderRequest(cam, request)) RenderPipeline.SubmitRenderRequest(cam, request);
            else cam.Render();
            var prior = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            tex.Apply();
            RenderTexture.active = prior;
            File.WriteAllBytes(Path.Combine(Out, file), tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        static void AppendStats(StringBuilder report, Scene scene)
        {
            long tris = 0; int renderers = 0, submeshes = 0;
            var materials = new HashSet<Material>(); var textures = new HashSet<Texture>(); var meshes = new HashSet<Mesh>();
            var groups = new Dictionary<string, long>();
            foreach (var root in scene.GetRootGameObjects())
            {
                long groupTris = 0;
                foreach (var r in root.GetComponentsInChildren<MeshRenderer>(false))
                {
                    var mf = r.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh == null || !r.enabled) continue;
                    var mesh = mf.sharedMesh; meshes.Add(mesh);
                    renderers++; submeshes += mesh.subMeshCount;
                    for (int s = 0; s < mesh.subMeshCount; s++) groupTris += mesh.GetIndexCount(s) / 3;
                    foreach (var m in r.sharedMaterials)
                    {
                        if (m == null) continue;
                        materials.Add(m);
                        foreach (var id in m.GetTexturePropertyNameIDs()) { var t = m.GetTexture(id); if (t != null) textures.Add(t); }
                    }
                }
                if (groupTris > 0) groups[root.name] = groupTris;
                tris += groupTris;
            }
            foreach (var g in groups) report.AppendLine($"- {g.Key}: {g.Value:N0} triangles");
            report.AppendLine($"- **Total (incl. the current sentence's stones): {tris:N0} triangles**");
            report.AppendLine($"- Mesh renderers {renderers}, sub-meshes {submeshes} (= draw calls before batching), unique materials {materials.Count}");
            long texMem = textures.Sum(t => Profiler.GetRuntimeMemorySizeLong(t));
            long meshMem = meshes.Sum(m => Profiler.GetRuntimeMemorySizeLong(m));
            report.AppendLine($"- Textures {textures.Count} ({texMem / (1024f * 1024f):0.0} MB in editor), meshes {meshes.Count} ({meshMem / (1024f * 1024f):0.0} MB in editor)");
            report.AppendLine("- Environment and dashboard are batching-static; expected draw calls after static batching are close to the material count.");
        }

        static T Get<T>(Scene scene) where T : Component
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var c = root.GetComponentInChildren<T>(true);
                if (c != null) return c;
            }
            throw new InvalidOperationException($"{typeof(T).Name} not found in {scene.path} — rebuild the integration scene.");
        }

        static string Yes(bool v) => v ? "yes" : "**NO**";
    }
}
