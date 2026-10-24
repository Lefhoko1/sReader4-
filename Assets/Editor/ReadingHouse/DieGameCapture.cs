using System;
using System.Collections;
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
    /// so key-word activities come up. It then plays: right picks, one wrong pick, flips, and every activity
    /// it meets solved piece by piece. Screenshots + LOG.txt in GeneratedAssets/ReadingHouseUnity/diegame.
    /// </summary>
    [InitializeOnLoad]
    public static class DieGameCapture
    {
        const string StepKey = "sReader.RH.Die.Step", ReturnKey = "sReader.RH.Die.Return", CountKey = "sReader.RH.Die.Count";
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
            SessionState.EraseInt("sReader.RH.Die.GateShot");
            SessionState.EraseInt("sReader.RH.Die.TierShot");
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

        static string Clean(string word)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in word) if (char.IsLetterOrDigit(c) || c == '\'' || c == '-') sb.Append(c);
            return sb.Length > 0 ? sb.ToString() : word;
        }

        /// <summary>The face carrying the right answer now (−1: not on the die, flip).</summary>
        static int RightFace(ReadingHousePresenter p, out string what)
        {
            var keys = (System.Collections.Generic.List<string>)Get(p, "faceKeys");
            var step = Get(p, "step").ToString();
            what = step;
            if (step == "Sentence")
            {
                var words = (System.Collections.Generic.List<(string text, int start, int end)>)Get(p, "words");
                int i = (int)Get(p, "buildIndex");
                var want = Clean(words[i].text); what += " word '" + want + "'";
                return keys.FindIndex(k => string.Equals(k, want, StringComparison.OrdinalIgnoreCase));
            }
            if (step == "Gate")
            {
                var gate = (GateData)Get(p, "dieGate");
                if (gate.Type == GateType.Illustrate)
                {
                    var token = (SReader.Domains.Assignments.Models.ContentToken)Get(p, "token");
                    what += " illustrate " + token.text;
                    return keys.IndexOf(token.correctImage);
                }
                var correct = (System.Collections.Generic.List<string>)Get(p, "correct");
                var answer = (System.Collections.Generic.List<string>)Get(p, "answer");
                var want = correct[answer.Count]; what += $" {gate.Type} piece '{want}' ({answer.Count + 1}/{correct.Count})";
                return keys.FindIndex(k => gate.Type == GateType.Define ? k == want : string.Equals(k, want, StringComparison.OrdinalIgnoreCase));
            }
            return -1;
        }

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            var p = Object.FindAnyObjectByType<ReadingHousePresenter>();
            void Wait(double s, int n) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StepKey, n); }
            void Shot(string name)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(Out, name + ".png"));
                Log($"SHOT {name}: step {Get(p, "step")}, sentence {p.SentenceIndex + 1}, streak {Get(p, "streak")}, tier {p.die.Tier_}, solved {p.Session?.SolvedGates}/{p.Session?.TotalGates}");
            }
            int count = SessionState.GetInt(CountKey, 0);

            switch (step)
            {
                case 0:
                {
                    // At the dashboard with the sample book (in memory only, for this check).
                    if (d != null) d.enabled = false;
                    var walker = d != null ? d.walker : null;
                    if (walker != null) walker.enabled = false;
                    if (d != null && d.follow != null) d.follow.Release();          // the reading camera takes over, as when reading starts
                    p.Bind(TrailBuilder.Build("sample", "Meadow at Dawn", AssignmentContentCodec.FromJson(p.sampleBook.text)));
                    p.EnterView(ReadingHouseView.Gameplay);
                    Log($"bound sample: {p.Trail.Sentences.Count} sentences, {p.Trail.Gates.Count} key words: " + string.Join(", ", p.Trail.Gates.ConvertAll(g => g.Type + " " + g.Word)));
                    Wait(2.5, 1); break;
                }
                case 1: Shot("01_die_at_dashboard"); Wait(0.3, 2); break;
                case 2:
                {
                    // One wrong pick first.
                    int right = RightFace(p, out var what);
                    int wrong = right == 0 ? 1 : 0;
                    Log($"wrong pick on face {wrong} ({what})");
                    p.OnDieTapped(wrong);
                    Wait(0.15, 3); break;
                }
                case 3: Shot("02_wrong_pick_shake"); Wait(0.6, 4); break;
                case 4:
                {
                    // Play on: right picks; flip when the right piece is not showing.
                    if (count > 160) { Wait(0.2, 90); break; }
                    var st = Get(p, "step").ToString();
                    if (st == "Done" && p.Session != null && p.Session.CanComplete && p.SentenceIndex >= p.Trail.Sentences.Count - 1) { Wait(1.6, 90); break; }   // passage complete
                    if (st == "Done" || st == "Off") { Wait(0.4, 4); break; }
                    if (p.die.Busy) { Wait(0.15, 4); break; }
                    int right = RightFace(p, out var what);
                    SessionState.SetInt(CountKey, count + 1);
                    if (right < 0) { Log($"flip ({what})"); p.OnDieFlip(1); Wait(0.6, 4); break; }
                    Log($"pick face {right}: {what}");
                    bool gate = st == "Gate";
                    p.OnDieTapped(right);
                    // shots at the interesting moments
                    if (count == 2) { Wait(0.7, 10); break; }
                    if (gate && SessionState.GetInt("sReader.RH.Die.GateShot", 0) < 4) { SessionState.SetInt("sReader.RH.Die.GateShot", SessionState.GetInt("sReader.RH.Die.GateShot", 0) + 1); Wait(0.7, 11); break; }
                    if (p.Session != null && p.Session.SolvedGates >= 3 && SessionState.GetInt("sReader.RH.Die.TierShot", 0) == 0) { SessionState.SetInt("sReader.RH.Die.TierShot", 1); Wait(1.2, 12); break; }
                    Wait(0.55, 4); break;
                }
                case 10: Shot("03_words_building"); Wait(0.2, 4); break;
                case 11: Shot($"04_key_word_activity_{SessionState.GetInt("sReader.RH.Die.GateShot", 0)}"); Wait(0.2, 4); break;
                case 12: Shot("05_precious_die_after_3_key_words"); Wait(0.2, 4); break;
                case 90: Shot("06_end"); Wait(0.5, 91); break;
                default: Stop(); break;
            }
        }

        static void Stop()
        {
            SessionState.EraseInt("sReader.RH.Die.GateShot");
            SessionState.EraseInt("sReader.RH.Die.TierShot");
            SessionState.SetInt(StepKey, 99);
            EditorApplication.isPlaying = false;
        }
    }
}
