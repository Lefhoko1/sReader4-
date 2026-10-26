using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;
using SReader.Game.Trek;
using SReader.Domains.Assignments.Models;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Automated check of the die game. Plays SC_ReadingHouse_Integration, puts the student at the dashboard
    /// and, for the check only, binds the bundled sample book in memory (not live: nothing is saved or sent),
    /// so key-word activities come up. Then it plays the passage as a student would: taps the glowing key word,
    /// waits for the self-shuffling die to show the right piece and taps it through the house's input (one
    /// wrong tap first), goes back to the passage, and so on until every key word is restored.
    /// Screenshots + LOG.txt in GeneratedAssets/ReadingHouseUnity/diegame.
    /// </summary>
    [InitializeOnLoad]
    public static class DieGameCapture
    {
        const string StepKey = "sReader.RH.Die.Step", ReturnKey = "sReader.RH.Die.Return", CountKey = "sReader.RH.Die.Count", ShotKey = "sReader.RH.Die.Shots";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "diegame"));
        static double next;
        static readonly BindingFlags F = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        static DieGameCapture()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += s =>
            {
                if (s == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(StepKey, -1) == 0) next = EditorApplication.timeSinceStartup + 4.0;
                if (s == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(StepKey, -1) == 99)
                {
                    SessionState.EraseInt(StepKey);
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
            SessionState.SetInt(CountKey, 0);
            SessionState.SetString(ShotKey, "");
            EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Single);
            SessionState.SetInt(StepKey, 0);
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            int step = SessionState.GetInt(StepKey, -1);
            if (step < 0 || step == 99 || !EditorApplication.isPlaying) return;
            if (EditorApplication.timeSinceStartup < next) return;
            try { Step(step); }
            catch (Exception e) { Log("ERROR " + e); Stop(); }
        }

        static void Log(string s) => File.AppendAllText(Path.Combine(Out, "LOG.txt"), s + "\n");
        static object Get(object o, string name) => o.GetType().GetField(name, F)?.GetValue(o);
        static void Call(object o, string name, params object[] args) => o.GetType().GetMethod(name, F).Invoke(o, args);

        /// <summary>Is the die showing the piece that comes next?</summary>
        static bool ShowingRight(ReadingHousePresenter p, out string what)
        {
            var keys = (System.Collections.Generic.List<string>)Get(p, "faceKeys");
            var gate = (GateData)Get(p, "dieGate");
            what = "";
            if (gate == null || keys == null || keys.Count == 0) return false;
            if (gate.Type == GateType.Illustrate)
            {
                var token = (ContentToken)Get(p, "token");
                what = "picture for " + token.text;
                return keys[0] == token.correctImage;
            }
            var correct = (System.Collections.Generic.List<string>)Get(p, "correct");
            var answer = (System.Collections.Generic.List<string>)Get(p, "answer");
            if (answer.Count >= correct.Count) return false;
            var want = correct[answer.Count];
            what = $"{gate.Type} piece '{want}' ({answer.Count + 1}/{correct.Count}), die shows '{keys[0]}'";
            return gate.Type == GateType.Define ? keys[0] == want : string.Equals(keys[0], want, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Tap the die the way a finger does: through the house's input, at the face showing.</summary>
        static void TapDie(ReadingHousePresenter p)
        {
            var input = Object.FindAnyObjectByType<ReadingHouseInput>();
            if (!p.die.TryGetChoicePoint(0, out var world)) { Log("  no face showing"); return; }
            var screen = (Vector2)p.cameraRig.viewCamera.WorldToScreenPoint(world);
            int before = (int)Get(p, "streak");
            input.Tap(screen);
            Log($"  tapped die at {screen}: streak {before} -> {Get(p, "streak")}");
        }

        static bool FirstTime(string tag)
        {
            var done = SessionState.GetString(ShotKey, "");
            if (done.Contains("|" + tag + "|")) return false;
            SessionState.SetString(ShotKey, done + "|" + tag + "|");
            return true;
        }

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            var p = Object.FindAnyObjectByType<ReadingHousePresenter>();
            void Wait(double s, int n) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StepKey, n); }
            void Shot(string name)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(Out, name + ".png"));
                Log($"SHOT {name}: playing {Get(p, "dieGate") != null}, solved {p.Session?.SolvedGates}/{p.Session?.TotalGates}, streak {Get(p, "streak")}, tier {p.die.Tier_}");
            }
            int count = SessionState.GetInt(CountKey, 0);
            SessionState.SetInt(CountKey, count + 1);
            if (count > 600) { Wait(0.2, 90); return; }

            switch (step)
            {
                case 0:
                {
                    // At the dashboard with the sample book (in memory only, for this check).
                    if (d != null) { d.enabled = false; if (d.walker != null) d.walker.enabled = false; if (d.follow != null) d.follow.Release(); }
                    p.Bind(TrailBuilder.Build("sample", "Meadow at Dawn", AssignmentContentCodec.FromJson(p.sampleBook.text)));
                    p.EnterView(ReadingHouseView.Gameplay);
                    Log($"bound sample: {p.Trail.Sentences.Count} sentences, {p.Trail.Gates.Count} key words: " + string.Join(", ", p.Trail.Gates.ConvertAll(g => g.Type + " " + g.Word)));
                    Wait(2.5, 1); break;
                }
                case 1: Shot("01_passage_with_key_words"); Wait(0.3, 4); break;
                case 4:
                {
                    var gate = (GateData)Get(p, "dieGate");
                    bool solvedShowing = (bool)Get(p, "solvedShowing");
                    if (gate == null)
                    {
                        if (p.Session.CanComplete) { Wait(1.0, 90); break; }
                        // after Start again: one key word played through proves the game still answers
                        if (SessionState.GetString(ShotKey, "").Contains("|retried|") && p.Session.SolvedGates >= 1) { Wait(0.5, 97); break; }
                        var target = p.Session.ActiveGate ?? p.Session.PeekNextGate();
                        int idx = p.Trail.Gates.IndexOf(target);
                        if (FirstTime("outoforder") && p.Trail.Gates.Count > 1)
                        {
                            int other = idx == 0 ? 1 : 0;
                            Log($"tap an out-of-order key word ({p.Trail.Gates[other].Word})");
                            Call(p, "OnKeyWordPressed", other);
                            Wait(1.0, 4); break;
                        }
                        Log($"tap key word {target.Word} ({target.Type})");
                        Call(p, "OnKeyWordPressed", idx);
                        Wait(1.2, FirstTime("play_" + target.Type) ? 11 : 4); break;
                    }
                    if (solvedShowing)
                    {
                        if (FirstTime("solved")) { Wait(0.6, 12); break; }
                        Call(p, "BackToPassage");
                        Wait(1.0, FirstTime("back") ? 13 : 4); break;
                    }
                    if (p.die.Busy) { Wait(0.15, 4); break; }
                    bool right = ShowingRight(p, out var what);
                    if (!right && FirstTime("wrongtap")) { Log("wrong tap: " + what); TapDie(p); Wait(0.25, 14); break; }
                    if (!right) { Wait(0.3, 4); break; }                       // the die shuffles itself: wait for the right piece
                    Log("right: " + what);
                    TapDie(p);
                    Wait(0.15, FirstTime("fly") ? 15 : 4); break;
                }
                case 11: Shot($"02_play_board_{((GateData)Get(p, "dieGate"))?.Type}"); Wait(0.2, 4); break;
                case 12: Shot("04_restored"); Wait(0.2, 4); break;
                case 13: Shot("05_back_to_passage"); Wait(0.2, 4); break;
                case 14: Shot("03_wrong_tap_shake"); Wait(0.3, 4); break;
                case 15: Shot("03b_piece_flying_to_slot"); Wait(0.5, 4); break;
                case 90: Shot("06_end"); Wait(0.5, 92); break;
                case 92:
                    Log("press Start again (confirmed)");
                    Call(p, "RetryAssignment");
                    Wait(1.5, 93); break;
                case 93: Shot("07_after_start_again"); FirstTime("retried"); Wait(0.3, 4); break;
                case 97: Shot("07b_played_after_start_again"); Wait(0.3, 94); break;
                case 94:
                    Log("press Other assignments");
                    if (d != null) { d.enabled = true; }
                    Call(p, "LeaveForAssignments");
                    Wait(6.0, 95); break;
                case 95: Shot("08_other_assignments_board"); Log($"board open {d?.BoardOpen}, state {d?.Current}"); Wait(0.5, 96); break;
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
