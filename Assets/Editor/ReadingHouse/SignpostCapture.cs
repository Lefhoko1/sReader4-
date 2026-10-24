using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Plays SC_ReadingHouse_Integration (real saved session) and uses the signpost outside the house:
    /// taps "Library" through the house's own input (ReadingHouseInput.Tap at the sign on screen), follows
    /// the student walking in and the chalkboard opening on the assignments, then "My Progress" and
    /// "Settings". Writes GeneratedAssets/ReadingHouseUnity/signpost/*.png + LOG.txt. Nothing is saved.
    /// </summary>
    [InitializeOnLoad]
    public static class SignpostCapture
    {
        const string StepKey = "sReader.RH.Sign.Step", ReturnKey = "sReader.RH.Sign.Return";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "signpost"));
        static double next, deadline;

        static SignpostCapture()
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

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            var input = Object.FindAnyObjectByType<ReadingHouseInput>();
            var cam = d.cameraRig.viewCamera;
            void Wait(double s, int n) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StepKey, n); }
            void Shot(string name)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(Out, name + ".png"));
                Log($"{name}: state {d.Current}, inside door unlocked {d.door.Unlocked}, board open {d.BoardOpen}, focus {d.BoardFocus:0.00}, student {d.walker.transform.position}");
            }
            bool BoardReady() => d.BoardOpen && d.BoardFocus >= 0.999f;
            void TapSign(SignpostButton.Kind kind)
            {
                var sign = Object.FindObjectsByType<SignpostButton>(FindObjectsSortMode.None).First(s => s.kind == kind);
                var label = sign.transform.Find("Label");
                var p = (Vector2)cam.WorldToScreenPoint(label != null ? label.position : sign.transform.position);
                Log($"tap {kind} at screen {p}");
                input.Tap(p);
            }

            switch (step)
            {
                case 0: Shot("01_outside_signpost"); TapSign(SignpostButton.Kind.Library); Wait(0.25, 1); break;
                case 1: Shot("02_library_sign_knocked"); Wait(2.5, 2); break;
                case 2: Shot("03_walking_in"); deadline = EditorApplication.timeSinceStartup + 25; Wait(0.5, 3); break;
                case 3: if (!BoardReady() && EditorApplication.timeSinceStartup < deadline) { Wait(0.5, 3); break; } Wait(2.5, 4); break;
                case 4: Shot("04_library_on_board"); d.CloseChalkboard(); Wait(2.5, 5); break;
                case 5: d.OnSignpost(SignpostButton.Kind.Progress); deadline = EditorApplication.timeSinceStartup + 15; Wait(0.5, 6); break;
                case 6: if (!BoardReady() && EditorApplication.timeSinceStartup < deadline) { Wait(0.5, 6); break; } Wait(3.5, 7); break;
                case 7: Shot("05_my_progress_on_board"); d.CloseChalkboard(); Wait(2.5, 8); break;
                case 8: d.OnSignpost(SignpostButton.Kind.Settings); deadline = EditorApplication.timeSinceStartup + 15; Wait(0.5, 9); break;
                case 9: if (!BoardReady() && EditorApplication.timeSinceStartup < deadline) { Wait(0.5, 9); break; } Wait(3.0, 10); break;
                case 10: Shot("06_settings_on_board"); Wait(0.5, 11); break;
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
