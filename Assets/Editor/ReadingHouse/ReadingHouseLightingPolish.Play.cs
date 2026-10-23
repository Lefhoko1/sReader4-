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
        static Button ReviewPoolChip(ReadingHousePresenter presenter) => presenter.dashboardScreen.Host.Q("gate-pool")
            .Children().OfType<Button>().First(b => b.enabledSelf && string.Equals(b.text, SessionState.GetString("RH.WordReview.Letter", ""), StringComparison.OrdinalIgnoreCase));
        static void PressReview(DashboardScreen screen, VisualElement element, bool down)
        {
            var point = screen.ScreenPointOf(element);
            var map = typeof(WorldUIScreen).GetMethod("ScreenToBoard", BindingFlags.Instance | BindingFlags.NonPublic);
            var board = (Vector2)map.Invoke(screen, new object[] { new Vector2(point.x, Screen.height - point.y) });
            if (float.IsNaN(board.x)) throw new InvalidOperationException("Review tap missed the dashboard surface.");
            var e = new Event { type = down ? EventType.MouseDown : EventType.MouseUp,
                mousePosition = board / screen.page.panelSettings.scale, button = 0, clickCount = 1 };
            using (var evt = down ? (EventBase)PointerDownEvent.GetPooled(e) : PointerUpEvent.GetPooled(e))
                screen.page.rootVisualElement.panel.visualTree.SendEvent(evt);
        }
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
                    case 6:
                        foreach (var ws in p.matStones.Live)
                        {
                            var label = ws.GetComponentInChildren<TMPro.TextMeshPro>();
                            var stoneSurface = ws.GetComponentInChildren<MeshRenderer>();
                            var camera = p.cameraRig.viewCamera;
                            var a = camera.WorldToScreenPoint(label.transform.position);
                            var b = camera.WorldToScreenPoint(stoneSurface.bounds.center);
                            if (Vector2.Distance(a, b) > 1f) throw new InvalidOperationException("Word is not centred over its stone: " + ws.word);
                        }
                        PlayLog("PASS all live word labels centred on their stones in screen space");
                        Save("03_reading"); Wait(7, .5); break;
                    case 7:
                        foreach (var stone in p.matStones.Live.Where(s => s.isKeyword && !s.solved).ToArray())
                        {
                            if (p.Session.ActiveGate != null && p.matStones.TryGetWord(stone, out int wi)
                                && (p.Session.ActiveGate.PassageCharIndex < p.Words[wi].start || p.Session.ActiveGate.PassageCharIndex >= p.Words[wi].end)) continue;
                            p.OnMatStoneTapped(stone);
                            if (KeywordCard.ActionOpen || p.AttemptUIOpen) break;
                        }
                        if (KeywordCard.ActionOpen) { Wait(70, 1); break; }
                        if (!p.AttemptUIOpen) { PlayLog("No current key stone opened an activity."); Stop(); break; }
                        Wait(8, 3); break;
                    case 70:
                        if (p.AttemptUIOpen) throw new InvalidOperationException("Activity opened before the keyword action.");
                        Save("06_keyword_card");
                        PlayLog("PASS keyword tap opens its card before the activity");
                        Wait(71, .5); break;
                    case 71:
                        var action = Object.FindObjectsByType<TMPro.TextMeshPro>(FindObjectsSortMode.None).Single(t => t.name == "Action Label");
                        PlayLog("Keyword action: " + action.text);
                        All<ReadingHouseInput>().Single().Tap(p.cameraRig.viewCamera.WorldToScreenPoint(action.transform.position));
                        if (!p.AttemptUIOpen || KeywordCard.ActionOpen) throw new InvalidOperationException("Keyword action did not replace its card with the activity.");
                        PlayLog("PASS keyword action tap opens the real activity");
                        Wait(8, 3); break;
                    case 8:
                        Save("04_activity");
                        var footer = p.dashboardScreen.Host.Q("gate-actions");
                        if (footer != null)
                        {
                            var hostBounds = p.dashboardScreen.Host.worldBound;
                            if (footer.worldBound.yMax > hostBounds.yMax || footer.worldBound.yMin < hostBounds.yMin)
                                throw new InvalidOperationException("Activity footer is outside the dashboard.");
                            PlayLog($"PASS activity footer visible: {footer.worldBound}, display {hostBounds}");
                        }
                        foreach (var ws in p.matStones.Live)
                        {
                            var text = ws.GetComponentInChildren<TMPro.TextMeshPro>();
                            PlayLog($"LABEL {ws.word} preferred={text.GetPreferredValues()} world={text.renderer.bounds.size} position={text.transform.position}");
                        }
                        Wait(SessionState.GetString(StageKey, "").StartsWith("word-") ? 80 : 9, .5); break;
                    case 80:
                        PlayLog("Real gates: " + string.Join(", ", p.Trail.Gates.Select(g => g.Word + ":" + g.Type + ":" + g.State)));
                        SessionState.SetInt("RH.WordReview.Sentence", p.SentenceIndex);
                        SessionState.SetInt("RH.WordReview.Points", p.Session.Points);
                        var pool = p.dashboardScreen.Host.Q("gate-pool");
                        var answer = p.dashboardScreen.Host.Q("gate-answer");
                        var gate = p.Session.ActiveGate;
                        if (gate != null && gate.Type == SReader.Game.Trek.GateType.Fill && gate.Word.Length > answer.childCount + 1)
                        {
                            string letter = gate.Word[answer.childCount].ToString();
                            var chip = pool.Children().OfType<Button>().First(b => b.enabledSelf && string.Equals(b.text, letter, StringComparison.OrdinalIgnoreCase));
                            SessionState.SetInt("RH.WordReview.Answers", answer.childCount);
                            p.dashboardScreen.Host.Q<ScrollView>("rh-activity-body").ScrollTo(chip);
                            SessionState.SetString("RH.WordReview.Letter", letter);
                            Wait(81, .5);
                        }
                        else Wait(85, .5);
                        break;
                    case 81:
                        PressReview(p.dashboardScreen, ReviewPoolChip(p), true); Wait(82, .15); break;
                    case 82:
                        PressReview(p.dashboardScreen, ReviewPoolChip(p), false); Wait(83, .5); break;
                    case 83:
                        var selectedRow = p.dashboardScreen.Host.Q("gate-answer");
                        if (selectedRow.childCount != SessionState.GetInt("RH.WordReview.Answers", 0) + 1)
                            throw new InvalidOperationException("Dashboard pointer did not select a letter.");
                        Save("08_activity_selected");
                        PlayLog("PASS letter selected through dashboard screen-to-panel mapping");
                        PressReview(p.dashboardScreen, selectedRow.Children().OfType<Button>().Last(), true);
                        Wait(84, .15); break;
                    case 84:
                        PressReview(p.dashboardScreen, p.dashboardScreen.Host.Q("gate-answer").Children().OfType<Button>().Last(), false);
                        Wait(85, .5); break;
                    case 85:
                        if (p.dashboardScreen.Host.Q("gate-answer").childCount != SessionState.GetInt("RH.WordReview.Answers", 0))
                            throw new InvalidOperationException("Selected letter could not be returned to its pool.");
                        PlayLog("PASS selected letter returned; no score awarded");
                        p.RequestExit();
                        var define = p.Trail.Gates.FindIndex(g => g.Type == SReader.Game.Trek.GateType.Define);
                        SessionState.SetInt("RH.WordReview.Define", define);
                        if (define < 0) { PlayLog("No Define activity in this real assignment; no replacement content created."); Wait(88, .5); break; }
                        p.ShowSentence(p.Trail.Gates[define].SentenceIndex, false);
                        Wait(86, .8); break;
                    case 86:
                        var dg = p.Trail.Gates[SessionState.GetInt("RH.WordReview.Define", -1)];
                        var ds = p.matStones.Live.First(stone => p.matStones.TryGetWord(stone, out int wi) && dg.PassageCharIndex >= p.Words[wi].start && dg.PassageCharIndex < p.Words[wi].end);
                        p.OnMatStoneTapped(ds);
                        Wait(87, .8); break;
                    case 87:
                        var defineLabel = Object.FindObjectsByType<TMPro.TextMeshPro>(FindObjectsSortMode.None).Single(t => t.name == "Action Label");
                        if (defineLabel.text != "Define" || p.AttemptUIOpen) throw new InvalidOperationException("Define keyword card missing.");
                        Save("07_define_card");
                        Wait(870, .5); break;
                    case 870:
                        p.RequestExit();
                        if (KeywordCard.ActionOpen) throw new InvalidOperationException("Back did not dismiss the keyword card.");
                        PlayLog("PASS real Define keyword card; Back dismisses without leaving reading");
                        Wait(88, .5); break;
                    case 88:
                        p.ShowSentence(SessionState.GetInt("RH.WordReview.Sentence", 0), false);
                        var key = p.matStones.Live.First(stone => stone.isKeyword);
                        p.OnMatStoneTapped(key);
                        Wait(89, .8); break;
                    case 89:
                        All<ReadingHouseInput>().Single().Tap(new Vector2(5, 5));
                        if (KeywordCard.ActionOpen || p.AttemptUIOpen) throw new InvalidOperationException("Outside tap did not dismiss only the keyword card.");
                        p.OnMatStoneTapped(p.matStones.Live.First(stone => stone.isKeyword));
                        p.ShowSentence(p.SentenceIndex, false);
                        if (KeywordCard.ActionOpen) throw new InvalidOperationException("Sentence refresh left a stale keyword action.");
                        if (p.Session.Points != SessionState.GetInt("RH.WordReview.Points", -1)) throw new InvalidOperationException("Review changed the score.");
                        PlayLog("PASS outside tap and sentence refresh dismiss the card; real score unchanged");
                        Wait(9, .5); break;
                    default: Stop(); break;
                }
            }
            catch (Exception ex) { PlayLog("ERROR " + ex); SessionState.SetInt(PlayKey, 99); EditorApplication.isPlaying = false; }
        }
    }
}
