using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SReader.Game.ReadingHouse;
using SReader.UI.Views.Home;
using SReader.UI.ViewModels;
using Object = UnityEngine.Object;
namespace SReader.EditorTools.ReadingHouse
{
    public static partial class ReadingHouseLightingPolish
    {
        const string PlayKey = "RH.Polish.CaptureStep", StageKey = "RH.Polish.Stage";
        static double playNext, authDeadline;
        static int resolutionRetries;
        static EditorWindow gameView;
        static ReadingHouseLightingPolishPlayHooks hooks = new ReadingHouseLightingPolishPlayHooks();
        sealed class ReadingHouseLightingPolishPlayHooks
        {
            public ReadingHouseLightingPolishPlayHooks()
            {
                EditorApplication.update += PlayTick;
                EditorApplication.playModeStateChanged += state => {
                    if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(PlayKey, -1) == 0) { SetCaptureSize(false); playNext = EditorApplication.timeSinceStartup + 5; authDeadline = playNext + 20; }
                    if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(PlayKey, -1) == 99) {
                        SessionState.EraseInt(PlayKey);
                        if (gameView != null) gameView.GetType().GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(gameView, SessionState.GetInt("RH.Polish.OldSize", 0));
                    }
                };
            }
        }
        static partial void StartPlayCapture(string stage)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || House().isDirty) throw new InvalidOperationException("Capture needs the saved house in Edit mode.");
            resolutionRetries = 0; SetCaptureSize(true);
            Directory.CreateDirectory(Output + "/" + stage + "-play");
            SessionState.SetString(StageKey, stage); SessionState.SetInt(PlayKey, 0);
            EditorApplication.isPlaying = true;
        }
        static void SetCaptureSize(bool remember)
        {
            var assembly = typeof(EditorWindow).Assembly;
            var gameType = assembly.GetType("UnityEditor.GameView");
            gameView = EditorWindow.GetWindow(gameType); gameView.Show();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var sizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null);
            var groupType = sizesType.GetProperty("currentGroupType", flags).GetValue(sizes);
            var group = sizesType.GetMethod("GetGroup", flags).Invoke(sizes, new[] { groupType });
            int count = (int)group.GetType().GetMethod("GetTotalCount", flags).Invoke(group, null), index = -1;
            for (int i = 0; i < count; i++) {
                var s = group.GetType().GetMethod("GetGameViewSize", flags).Invoke(group, new object[] { i });
                if ((int)s.GetType().GetProperty("width", flags).GetValue(s) == 1520 && (int)s.GetType().GetProperty("height", flags).GetValue(s) == 720) { index = i; break; }
            }
            if (index < 0) {
                var sizeType = assembly.GetType("UnityEditor.GameViewSize");
                var kind = assembly.GetType("UnityEditor.GameViewSizeType");
                var size = Activator.CreateInstance(sizeType, flags, null, new object[] { Enum.Parse(kind, "FixedResolution"), 1520, 720, "Reading House polish" }, null);
                group.GetType().GetMethod("AddCustomSize", flags).Invoke(group, new[] { size }); index = count;
            }
            var selected = gameType.GetProperty("selectedSizeIndex", flags);
            if (remember) SessionState.SetInt("RH.Polish.OldSize", (int)selected.GetValue(gameView)); selected.SetValue(gameView, 0); selected.SetValue(gameView, index); gameView.Repaint();
        }
        static void PlayLog(string message) => File.AppendAllText(Output + "/" + SessionState.GetString(StageKey, "unknown") + "-play/log.txt", message + "\n");
        static void PlayTick()
        {
            int step = SessionState.GetInt(PlayKey, -1);
            if (step < 0 || step == 99 || !EditorApplication.isPlaying) return;
            if (gameView == null) gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            gameView.Repaint();
            if (EditorApplication.timeSinceStartup < playNext) return;
            try
            {
                if (step == 0 && (Screen.width != 1520 || Screen.height != 720)) {
                    if (++resolutionRetries > 20) throw new InvalidOperationException($"Game view did not settle at 1520x720: {Screen.width}x{Screen.height}");
                    SetCaptureSize(false); playNext = EditorApplication.timeSinceStartup + .5; return;
                }
                var d = All<ReadingHouseExploreDirector>().Single(); var p = d.presenter;
                void Wait(int n, double s) { SessionState.SetInt(PlayKey, n); playNext = EditorApplication.timeSinceStartup + s; }
                void Save(string name) {
                    if (Screen.width != 1520 || Screen.height != 720) throw new InvalidOperationException($"Expected 1520x720, actual {Screen.width}x{Screen.height}");
                    ScreenCapture.CaptureScreenshot(Output + "/" + SessionState.GetString(StageKey, "unknown") + "-play/" + name + ".png");
                    PlayLog($"{name} signedIn={d.door.Unlocked} sampleAllowed={p.allowSampleContent} batches/drawCalls={UnityStats.drawCalls} setPass={UnityStats.setPassCalls} triangles={UnityStats.triangles} editorFrame={Time.unscaledDeltaTime * 1000:F2}ms");
                }
                void Stop() { SessionState.SetInt(PlayKey, 99); EditorApplication.isPlaying = false; }
                switch (step) {
                    case 0: Save("01_exterior"); Wait(1, .5); break;
                    case 1:
                        if (!d.door.Unlocked) {
                            if (EditorApplication.timeSinceStartup < authDeadline) { Wait(1, .5); break; }
                            PlayLog("No signed-in session restored. Interior review uses editor previews; live dashboard activity and chalkboard pages cannot be verified. No sample data loaded."); Stop(); break;
                        }
                        d.walker.enabled = false; d.walker.transform.position = p.studentMover.overviewSpot.position; d.walker.enabled = true; Wait(2, 3); break;
                    case 2:
                        Save("02_follow_inside");
                        var point = d.chalkboard.position + Vector3.ProjectOnPlane(d.door.houseCentre.position - d.chalkboard.position, Vector3.up).normalized * 2.2f;
                        point.y = p.studentMover.gameplaySpot.position.y;
                        d.walker.enabled = false; d.walker.transform.position = point; d.walker.enabled = true; Wait(3, 2); break;
                    case 3: d.OnChalkboardTapped(); Wait(4, 9); break;
                    case 4: Save("05_chalkboard"); Wait(5, 1); break;
                    case 5:
                        var home = d.chalkboardScreen.page.GetComponent<StudentHomeController>();
                        StudentAssignmentsViewModel vm = null;
                        for (var t = home.GetType(); t != null; t = t.BaseType) {
                            var prop = t.GetProperty("Assignments", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (prop != null) { vm = prop.GetValue(home) as StudentAssignmentsViewModel; break; }
                        }
                        var pick = vm?.MyAssignments.FirstOrDefault(v => !v.Submitted && v.Assignment != null && v.Assignment.HasContent);
                        if (pick == null) { PlayLog("No real readable unsubmitted assignment available; reading/activity not opened."); Stop(); break; }
                        d.OnAttemptStarted(pick, vm); Wait(6, 16); break;
                    case 6: Save("03_reading"); Wait(7, .5); break;
                    case 7:
                        foreach (var stone in p.matStones.Live.ToArray()) { p.OnMatStoneTapped(stone); if (p.AttemptUIOpen) break; }
                        if (!p.AttemptUIOpen) { PlayLog("No current key stone opened an activity."); Stop(); break; }
                        Wait(8, 3); break;
                    case 8: Save("04_activity"); Wait(9, .5); break;
                    default: Stop(); break;
                }
            }
            catch (Exception ex) { PlayLog("ERROR " + ex); SessionState.SetInt(PlayKey, 99); EditorApplication.isPlaying = false; }
        }
    }
}
