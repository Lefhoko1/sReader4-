using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.Game.ReadingHouse;
using SReader.UI.ViewModels;
using SReader.UI.Views.Home;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Plays SC_ReadingHouse_Integration with the REAL signed-in session (no sample data) and checks
    /// the chalkboard: walks the student up to it, taps it (the camera turns to it and the app's
    /// student home opens on it), clicks the bottom-nav buttons ON the board with a simulated mouse
    /// (so the screen → board tap mapping is tested for real), then picks the first readable
    /// assignment from the database through ReadingHouseLauncher — the same call the page's
    /// "🏠 Reading House" button makes — and starts reading it.
    /// If nobody is signed in it stops and says so. Writes GeneratedAssets/ReadingHouseUnity/chalkboard/.
    /// </summary>
    [InitializeOnLoad]
    public static class ChalkboardCapture
    {
        const string StepKey = "sReader.RH.CB.Step", ReturnKey = "sReader.RH.CB.Return", BgKey = "sReader.RH.CB.Bg";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "chalkboard"));
        static double next, startedAt;
        static EditorWindow gameView;

        static ChalkboardCapture()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += s =>
            {
                if (s == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(StepKey, -1) == 0) { next = EditorApplication.timeSinceStartup + 2.5; startedAt = next; }
                if (s == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(StepKey, -1) == 99)
                {
                    SessionState.EraseInt(StepKey);
                    PlayerSettings.runInBackground = SessionState.GetBool(BgKey, false);
                    var back = SessionState.GetString(ReturnKey, "");
                    if (!string.IsNullOrEmpty(back) && File.Exists(back)) EditorSceneManager.OpenScene(back, OpenSceneMode.Single);
                }
            };
        }

        public static void Start()
        {
            Directory.CreateDirectory(Out);
            foreach (var f in Directory.GetFiles(Out)) File.Delete(f);
            SessionState.SetString(ReturnKey, SceneManager.GetActiveScene().path);
            SessionState.SetBool(BgKey, PlayerSettings.runInBackground);
            PlayerSettings.runInBackground = true;
            EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Single);
            SessionState.SetInt(StepKey, 0);
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            int step = SessionState.GetInt(StepKey, -1);
            if (step < 0 || step == 99 || !EditorApplication.isPlaying) return;
            if (gameView == null)
            {
                var all = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                gameView = all.Length > 0 ? (EditorWindow)all[0] : null;
            }
            gameView?.Repaint();
            if (EditorApplication.timeSinceStartup < next) return;
            try { Step(step); }
            catch (Exception e) { Log("ERROR " + e); Stop(); }
        }

        static void Log(string line) => File.AppendAllText(Path.Combine(Out, "LOG.txt"), line + "\n");

        static StudentHomeController Home(ReadingHouseExploreDirector d) => d.chalkboardScreen.page.GetComponent<StudentHomeController>();

        static StudentAssignmentsViewModel AssignmentsVm(StudentHomeController home)
        {
            for (var t = home.GetType(); t != null; t = t.BaseType)
            {
                var p = t.GetProperty("Assignments", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (p != null) return p.GetValue(home) as StudentAssignmentsViewModel;
            }
            return null;
        }

        /// <summary>
        /// A tap on a page element through the board: the element's spot on screen → the board's own
        /// screen-to-board mapping (the function UI Toolkit calls for every real pointer) → a pointer
        /// down / up delivered to the page at that point. (Simulated devices don't reach the game in the
        /// editor — the real cursor overrides them — so the pointer is delivered at the panel.)
        /// </summary>
        static void Press(ReadingHouseExploreDirector d, string elementName, bool down)
        {
            var s = d.chalkboardScreen;
            var el = s.page.rootVisualElement.Q(elementName) ?? throw new InvalidOperationException($"No '{elementName}' on the board.");
            var screen = s.ScreenPointOf(el);                                  // bottom-left origin
            var map = typeof(WorldUIScreen).GetMethod("ScreenToBoard", BindingFlags.Instance | BindingFlags.NonPublic);
            var board = (Vector2)map.Invoke(s, new object[] { new Vector2(screen.x, Screen.height - screen.y) });
            var panelPos = board / s.page.panelSettings.scale;
            var picked = float.IsNaN(board.x) ? null : s.page.rootVisualElement.panel.Pick(panelPos);
            if (down) Log($"tap '{elementName}' at screen {screen} -> board {board} -> picks '{picked?.name ?? "nothing"}'");
            if (float.IsNaN(board.x)) return;
            var e = new Event { type = down ? EventType.MouseDown : EventType.MouseUp, mousePosition = panelPos, button = 0, clickCount = 1 };
            using (var evt = down ? (EventBase)PointerDownEvent.GetPooled(e) : PointerUpEvent.GetPooled(e))
                s.page.rootVisualElement.panel.visualTree.SendEvent(evt);
        }

        /// <summary>Like Press, for an element of the topmost active page on the board (e.g. Profile).</summary>
        static void PressOn(ReadingHouseExploreDirector d, string elementName, bool down)
        {
            var s = d.chalkboardScreen;
            var top = s.extraPages.FirstOrDefault(p => p != null && p.isActiveAndEnabled) ?? s.page;
            var el = top.rootVisualElement.Q(elementName) ?? throw new InvalidOperationException($"No '{elementName}' on '{top.name}'.");
            var screen = s.ScreenPointOf(el);
            var map = typeof(WorldUIScreen).GetMethod("ScreenToBoard", BindingFlags.Instance | BindingFlags.NonPublic);
            var board = (Vector2)map.Invoke(s, new object[] { new Vector2(screen.x, Screen.height - screen.y) });
            var panelPos = board / s.page.panelSettings.scale;
            if (down) Log($"tap '{elementName}' on {top.name} -> picks '{top.rootVisualElement.panel.Pick(panelPos)?.name ?? "nothing"}'");
            var e = new Event { type = down ? EventType.MouseDown : EventType.MouseUp, mousePosition = panelPos, button = 0, clickCount = 1 };
            using (var evt = down ? (EventBase)PointerDownEvent.GetPooled(e) : PointerUpEvent.GetPooled(e))
                top.rootVisualElement.panel.visualTree.SendEvent(evt);
        }

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            void Wait(double s, int n) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StepKey, n); }
            void Shot(string name)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(Out, name + ".png"));
                Log($"{name}: state {d.Current}, board open {d.BoardOpen}, signed in {d.door.Unlocked}, " +
                    $"assignment {d.presenter.LiveAssignment?.Assignment?.Title ?? "-"}");
            }

            switch (step)
            {
                case 0:
                    // Real sign-in only: wait for the saved session to restore (it opens the doors).
                    if (!d.door.Unlocked)
                    {
                        if (EditorApplication.timeSinceStartup - startedAt < 12) { Wait(0.5, 0); break; }
                        Log("NOT SIGNED IN — sign in at the door once (the session is saved), then run this again. No sample data is used.");
                        Stop(); break;
                    }
                    {
                        var chalk = d.chalkboard.position;
                        var intoRoom = Vector3.ProjectOnPlane(d.door.houseCentre.position - chalk, Vector3.up).normalized;
                        var spot = chalk + intoRoom * 2.2f;
                        spot.y = d.presenter.studentMover.gameplaySpot.position.y;
                        d.walker.enabled = false; d.walker.transform.position = spot; d.walker.enabled = true;
                        d.joystick.Simulated = new Vector2(0.4f, 0f);
                    }
                    Wait(0.5, 1); break;
                case 1: d.joystick.Simulated = null; Wait(1.5, 2); break;
                case 2: Shot("C1_room_before_board"); Wait(0.4, 21); break;
                case 21: d.OnChalkboardTapped(); Wait(0.45, 22); break;   // the room turns; the home page loads from the database
                case 22: Shot("C2a_turning"); Wait(0.5, 23); break;
                case 23: Shot("C2b_turning"); Wait(3.0, 3); break;
                case 3: Shot("C2_board_student_home"); Wait(0.4, 31); break;
                case 31: Press(d, "nav-academies", true); Wait(0.15, 32); break;
                case 32: Press(d, "nav-academies", false); Wait(3.5, 4); break;
                case 4: Shot("C3_board_academies"); Wait(0.4, 41); break;
                case 41: Press(d, "nav-assignments", true); Wait(0.15, 42); break;
                case 42: Press(d, "nav-assignments", false); Wait(4.0, 5); break;
                case 5: Shot("C4_board_assignments"); Wait(0.4, 52); break;
                case 52: Press(d, "profile-button", true); Wait(0.15, 53); break;
                case 53: Press(d, "profile-button", false); Wait(3.5, 54); break;
                case 54: Shot("C4b_board_profile"); Wait(0.4, 55); break;
                case 55: PressOn(d, "back-button", true); Wait(0.15, 56); break;
                case 56: PressOn(d, "back-button", false); Wait(8.0, 57); break;   // Back asks the database for the role first
                case 57: Shot("C4c_board_back_home"); Wait(0.4, 51); break;
                case 51:
                {
                    var vm = AssignmentsVm(Home(d));
                    var pick = vm?.MyAssignments.FirstOrDefault(v => !v.Submitted && v.Assignment != null && v.Assignment.HasContent);
                    Log($"database: {vm?.MyAssignments.Count ?? 0} assignments; readable & not submitted: {pick?.Assignment.Title ?? "none"}");
                    if (pick == null) { Stop(); break; }
                    // What the page's "Play solo" does (StudentAssignmentsView.LaunchGame → ClassroomLaunch.TryOpen).
                    bool handled = SReader.Game.Classroom.ClassroomLaunch.TryOpen(pick, vm, null);
                    Log($"Play solo handled by the house: {handled}");
                    Wait(2.2, 6); break;
                }
                case 6: Shot("C5_walking_to_dashboard"); Wait(0.5, 61); break;
                case 61:   // wait until he has walked there and the reading view is open
                    if (d.Current != ReadingHouseExploreDirector.State.Reading && EditorApplication.timeSinceStartup - startedAt < 120) { Wait(0.5, 61); break; }
                    Wait(2.8, 7); break;
                case 7: Shot("C6_reading_attempt_at_dashboard"); Wait(0.4, 71); break;
                case 71:
                {
                    // Tap the stones until the word the session is at opens its activity (others nudge / highlight).
                    Log($"stones on the rug: {d.presenter.matStones?.Live.Count ?? 0}");
                    foreach (var stone in d.presenter.matStones.Live.ToArray())
                    {
                        d.presenter.OnMatStoneTapped(stone);
                        if (d.presenter.AttemptUIOpen) { Log($"activity opened from stone '{stone.word}'"); break; }
                    }
                    if (!d.presenter.AttemptUIOpen) Log("no key word on this page opened an activity");
                    Wait(1.6, 72); break;
                }
                case 72: Shot("C7_word_activity_open"); Wait(0.4, 73); break;
                case 73: d.presenter.RequestExit(); Wait(1.2, 74); break;     // Back closes the activity first
                case 74: Shot("C8_back_at_dashboard"); Log($"activity open after Back: {d.presenter.AttemptUIOpen}"); Wait(0.5, 8); break;
                default: Stop(); break;
            }
        }

        static void Stop()
        {
            SessionState.SetInt(StepKey, 99);
            EditorApplication.isPlaying = false;
        }
    }
}
