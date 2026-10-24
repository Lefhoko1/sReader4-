using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SReader.Domains.Assignments.Models;
using SReader.Game.ReadingHouse;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;
using SReader.UI.Views.Home;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Milestone A live check. Plays the real app (SampleScene), lets it restore the
    /// signed-in session saved in this editor, loads the student's assignments
    /// through the EXISTING StudentAssignmentsViewModel, opens the Reading House
    /// exactly like the "🏠 Reading House" button does, and compares what the
    /// dashboard shows with the view model's data. Then presses Back, checks the
    /// app returned cleanly, stops Play mode and reopens the previously open scene.
    /// Read-only: it starts no attempt and saves no progress.
    /// </summary>
    [InitializeOnLoad]
    public static class ReadingHouseLiveTest
    {
        const string StateKey = "sReader.RH.LiveTest.State";
        const string ReturnSceneKey = "sReader.RH.LiveTest.ReturnScene";
        const string RunInBackgroundKey = "sReader.RH.LiveTest.RunInBackground";
        const string AppScene = "Assets/Scenes/SampleScene.unity";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "livetest"));

        static double deadline, waitUntil;
        static Task<SReader.Core.Common.Result> loadTask;
        static StudentAssignmentsViewModel vm;
        static AssignmentView chosen;
        static readonly StringBuilder report = new StringBuilder();

        static ReadingHouseLiveTest()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem("Tools/sReader/Reading House/Run Live Test (Play Mode)")]
        public static void Start()
        {
            Directory.CreateDirectory(Out);
            File.Delete(Path.Combine(Out, "LIVE_TEST_REPORT.md"));
            SessionState.SetString(ReturnSceneKey, SceneManager.GetActiveScene().path);
            // Play mode pauses while the editor is unfocused unless this is on; restored afterwards.
            SessionState.SetBool(RunInBackgroundKey, PlayerSettings.runInBackground);
            PlayerSettings.runInBackground = true;
            EditorSceneManager.OpenScene(AppScene, OpenSceneMode.Single);
            SessionState.SetString(StateKey, "await-play");
            EditorApplication.isPlaying = true;
        }

        static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode && SessionState.GetString(StateKey, "") == "await-play")
            {
                report.Clear();
                report.AppendLine("# Reading House — Milestone A live test");
                report.AppendLine($"Run {DateTime.Now:yyyy-MM-dd HH:mm} in Play mode on SampleScene with this editor's saved login.\n");
                deadline = EditorApplication.timeSinceStartup + 60;
                SessionState.SetString(StateKey, "await-vm");
            }
            if (change == PlayModeStateChange.EnteredEditMode && SessionState.GetString(StateKey, "") == "finishing")
            {
                SessionState.EraseString(StateKey);
                PlayerSettings.runInBackground = SessionState.GetBool(RunInBackgroundKey, false);
                AssetDatabase.SaveAssets();
                var back = SessionState.GetString(ReturnSceneKey, "");
                if (!string.IsNullOrEmpty(back) && File.Exists(back)) EditorSceneManager.OpenScene(back, OpenSceneMode.Single);
            }
        }

        static void Tick()
        {
            string state = SessionState.GetString(StateKey, "");
            if (string.IsNullOrEmpty(state) || !EditorApplication.isPlaying || EditorApplication.timeSinceStartup < waitUntil) return;
            try { Step(state); }
            catch (Exception e) { Finish("FAILED with an exception:\n```\n" + e + "\n```"); }
        }

        static void Step(string state)
        {
            if (EditorApplication.timeSinceStartup > deadline) { Finish($"FAILED: timed out in state '{state}'."); return; }
            switch (state)
            {
                case "await-vm":
                {
                    var home = Object.FindAnyObjectByType<StudentHomeController>(FindObjectsInactive.Include);
                    if (home == null) return;
                    vm = typeof(StudentHomeController).BaseType?
                        .GetProperty("Assignments", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(home) as StudentAssignmentsViewModel;
                    if (vm == null) return;
                    if (string.IsNullOrEmpty(vm.CurrentUserId)) return;   // session restore still running
                    report.AppendLine($"- Signed-in user id: `{vm.CurrentUserId}` (restored session)");
                    loadTask = vm.LoadAsync();
                    SessionState.SetString(StateKey, "await-load");
                    return;
                }
                case "await-load":
                {
                    if (!loadTask.IsCompleted) return;
                    var r = loadTask.Result;
                    if (r.IsFailure) { Finish("FAILED: StudentAssignmentsViewModel.LoadAsync → " + r.Error); return; }
                    var all = vm.MyAssignments;
                    report.AppendLine($"- Assignments loaded from Supabase via StudentAssignmentsViewModel.LoadAsync: **{all.Count}**");
                    chosen = all.FirstOrDefault(v => !v.Submitted && v.Assignment.HasContent && vm.LoadProgress(v.Assignment.Id) != null)
                          ?? all.FirstOrDefault(v => !v.Submitted && v.Assignment.HasContent)
                          ?? all.FirstOrDefault(v => v.Assignment.HasContent);
                    if (chosen == null) { Finish("STOPPED: this account has no assignment with reading content to open."); return; }
                    report.AppendLine($"- Opened: **{chosen.Assignment.Title}** (`{chosen.Assignment.Id}`), status {chosen.Status}, class {vm.ClassNameFor(chosen.Assignment.ClassId)}");
                    ReadingHouseLauncher.Launch(chosen, vm);
                    waitUntil = EditorApplication.timeSinceStartup + 3.5;   // scene load + overview → gameplay glide
                    SessionState.SetString(StateKey, "check");
                    return;
                }
                case "check":
                {
                    var presenter = Object.FindAnyObjectByType<ReadingHousePresenter>();
                    if (presenter == null) { Finish("FAILED: the Reading House scene did not load (is it in Build Settings?)."); return; }
                    Check(presenter);
                    Capture(presenter, "live_gameplay.png");
                    presenter.EnterView(ReadingHouseView.Overview);
                    waitUntil = EditorApplication.timeSinceStartup + 1.5;
                    SessionState.SetString(StateKey, "overview");
                    return;
                }
                case "overview":
                {
                    var presenter = Object.FindAnyObjectByType<ReadingHousePresenter>();
                    Capture(presenter, "live_overview.png");
                    presenter.RequestExit();   // same path as the Back button
                    waitUntil = EditorApplication.timeSinceStartup + 1.5;
                    SessionState.SetString(StateKey, "exit");
                    return;
                }
                case "exit":
                {
                    bool unloaded = !SceneManager.GetSceneByName(ReadingHouseLauncher.SceneName).isLoaded;
                    var homePage = Object.FindAnyObjectByType<StudentHomeController>(FindObjectsInactive.Include);
                    bool homeShown = homePage != null && homePage.gameObject.activeInHierarchy;
                    report.AppendLine($"\n## Back\n- Reading House scene unloaded: {Yes(unloaded)}\n- Student Home shown again: {Yes(homeShown)}");
                    Finish(unloaded && homeShown ? "PASSED" : "FAILED: Back did not return cleanly.");
                    return;
                }
            }
        }

        static void Check(ReadingHousePresenter p)
        {
            var a = chosen.Assignment;
            var trail = p.Trail; var session = p.Session; var dash = p.dashboard;
            var progress = vm.LoadProgress(a.Id);
            int expectedSentence = session.CurrentSentenceIndex;
            var span = trail.Sentences.Count > 0 ? trail.Sentences[expectedSentence] : null;
            string expectedText = span != null ? trail.PassageText.Substring(span.Start, span.Length).Trim() : "";
            int stones = p.tray.Active.Count;

            report.AppendLine("\n## Dashboard vs. existing runtime data\n");
            report.AppendLine("| Field | Shown on the dashboard | Source of truth | Match |");
            report.AppendLine("|---|---|---|---|");
            Row("Assignment title", dash.assignmentTitle.text, a.Title + " (Assignment.Title)", dash.assignmentTitle.text == a.Title);
            string exp = $"Sentence {expectedSentence + 1} of {trail.Sentences.Count}";
            Row("Sentence progress", dash.sentenceProgress.text, exp + " (TrekSession.CurrentSentenceIndex / TrailData.Sentences)", dash.sentenceProgress.text == exp);
            float fill = dash.progressFill.localScale.x, expFill = trail.Sentences.Count > 0 ? (expectedSentence + 1f) / trail.Sentences.Count : 0f;
            Row("Progress bar fill", fill.ToString("0.00"), expFill.ToString("0.00"), Mathf.Abs(fill - expFill) < 0.01f);
            Row("Sentence on display", Trunc(dash.sentenceText.text), Trunc(expectedText) + " (TrailData from vm.ContentFor)", dash.sentenceText.text == expectedText);
            Row("Points", dash.pointsValue.text, $"{session.Points} (TrekSession.Points, restored from GameProgress score {progress?.Score.ToString() ?? "none"})", dash.pointsValue.text == session.Points.ToString());
            Row("Word stones", stones.ToString(), p.Words.Count + " words in that sentence", stones == p.Words.Count);
            report.AppendLine($"\n- Key words in the assignment: {trail.Gates.Count}; solved per the saved resume point: {session.SolvedGates}");
            report.AppendLine($"- Live mode: {Yes(p.IsLive)}; placeholder dialogue hidden: {Yes(!dash.encouragerSpeech.gameObject.activeSelf)}");
        }

        static void Row(string field, string shown, string truth, bool ok) => report.AppendLine($"| {field} | {shown} | {truth} | {Yes(ok)} |");
        static string Trunc(string s) => s.Length > 70 ? s.Substring(0, 70) + "…" : s;
        static string Yes(bool v) => v ? "yes" : "**NO**";

        static void Capture(ReadingHousePresenter p, string file)
        {
            var cam = p.cameraRig.viewCamera;
            var rt = new RenderTexture(1520, 720, 24);
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt };
            if (RenderPipeline.SupportsRenderRequest(cam, request)) RenderPipeline.SubmitRenderRequest(cam, request);
            var prior = RenderTexture.active; RenderTexture.active = rt;
            var tex = new Texture2D(1520, 720, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 1520, 720), 0, 0); tex.Apply();
            RenderTexture.active = prior;
            File.WriteAllBytes(Path.Combine(Out, file), tex.EncodeToPNG());
            Object.DestroyImmediate(tex); rt.Release(); Object.DestroyImmediate(rt);
        }

        static void Finish(string verdict)
        {
            report.AppendLine($"\n## Result\n{verdict}");
            Directory.CreateDirectory(Out);
            File.WriteAllText(Path.Combine(Out, "LIVE_TEST_REPORT.md"), report.ToString());
            SessionState.SetString(StateKey, "finishing");
            EditorApplication.isPlaying = false;
        }
    }
}
