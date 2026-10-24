using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Game.ReadingHouse;
using Object = UnityEngine.Object;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Plays SC_ReadingHouse_Integration and walks the 2D student through the
    /// real controls (the joystick, driven by the test) in every direction,
    /// screenshotting each, then into the house and the reading view. Writes
    /// GeneratedAssets/ReadingHouseUnity/student2d/*.png and a log of which
    /// clip and facing the animator chose at each step. Nothing is saved.
    /// </summary>
    [InitializeOnLoad]
    public static class Student2DPlayCapture
    {
        const string StepKey = "sReader.RH.S2D.Step", ReturnKey = "sReader.RH.S2D.Return", BgKey = "sReader.RH.S2D.Bg";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "student2d"));
        static double next;
        static EditorWindow gameView;

        static Student2DPlayCapture()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += s =>
            {
                if (s == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(StepKey, -1) == 0) next = EditorApplication.timeSinceStartup + 2.5;
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
            catch (Exception e) { File.AppendAllText(Path.Combine(Out, "LOG.txt"), "ERROR " + e + "\n"); Stop(); }
        }

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            var anim = Object.FindAnyObjectByType<StudentSpriteAnimator>();
            var stick = d.joystick;
            var student = d.walker.transform;
            void Wait(double s, int n) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StepKey, n); }
            void Shot(string name)
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(Out, name + ".png"));
                if (anim == null || d.cameraRig == null || d.cameraRig.viewCamera == null || anim.spriteRenderer == null)
                    throw new InvalidOperationException($"missing: anim {anim == null}, rig {d.cameraRig == null}, cam {d.cameraRig?.viewCamera == null}, sr {anim?.spriteRenderer == null}");
                var camF = Vector3.ProjectOnPlane(d.cameraRig.viewCamera.transform.forward, Vector3.up).normalized;
                File.AppendAllText(Path.Combine(Out, "LOG.txt"),
                    $"{name}: facing {anim.CurrentFacing}, walking {anim.IsWalking}, sprite {anim.spriteRenderer.sprite?.name}, flipX {anim.spriteRenderer.flipX}, " +
                    $"pos {student.position}, stick {stick.Value}, walker on {d.walker.enabled}, cam fwd {camF}, state {d.Current}, door open {d.door.IsOpen}\n");
            }
            void Walk(Vector2 dir) { stick.Simulated = dir; }
            void Rec(string prefix, int n)
            {
                var r = new GameObject("Recorder").AddComponent<PlayModeFrameRecorder>();
                r.enabled = false; r.folder = Path.Combine(Out, "seq"); r.prefix = prefix; r.frames = n; r.fps = 20; r.enabled = true;
            }
            // any recorder still going (finished ones stay in the scene, so don't just look at the first one found)
            bool Recording() => System.Array.Exists(Object.FindObjectsByType<PlayModeFrameRecorder>(FindObjectsSortMode.None), r => !r.Finished);

            switch (step)
            {
                case 0: Shot("01_outside_idle_front"); Walk(new Vector2(1f, 0f)); Wait(0.9, 1); break;
                case 1: Shot("02_walk_right"); Walk(new Vector2(-1f, 0f)); Wait(1.2, 2); break;
                case 2: Shot("03_walk_left"); Walk(new Vector2(0f, -1f)); Wait(0.7, 3); break;
                case 3: Shot("04_walk_toward_camera"); Walk(new Vector2(0f, 1f)); Wait(1.6, 4); break;
                case 4: Shot("05_walk_away"); Walk(Vector2.zero); Wait(1.2, 5); break;
                case 5: Shot("06_stopped_facing_away_idle_back"); d.door.Unlock(); Walk(new Vector2(0f, 1f)); Wait(0.4, 6); break;
                case 6:
                {
                    // Put him at the door so the walk into the house is short, then walk in.
                    var outward = Vector3.ProjectOnPlane(d.door.transform.position - d.door.houseCentre.position, Vector3.up).normalized;
                    d.walker.enabled = false; student.position = d.door.transform.position + outward * 1.6f; d.walker.enabled = true;
                    Walk(new Vector2(0f, 1f)); Wait(2.4, 7); break;
                }
                case 7: Shot("07_walking_into_house"); Walk(Vector2.zero); Wait(1.0, 8); break;
                case 8: Shot("08_inside_idle"); Walk(Vector2.zero); d.TryStartReading(); Wait(2.4, 9); break;
                case 9: Shot("09_reading_view_back_at_dashboard"); anim.PlayOnce(anim.set.pointBack); Wait(0.5, 10); break;
                case 10: Shot("10_point_at_dashboard"); Wait(0.6, 20); break;
                case 20:
                {
                    // the Encourager: claps when a word is picked, cheers when one is solved
                    var stone = Object.FindAnyObjectByType<WordStoneView>();
                    if (stone != null) d.presenter.OnStoneTapped(stone);
                    Rec("seq_enc_clap", 30); Wait(0.3, 21); break;
                }
                case 21: if (Recording()) { Wait(0.3, 21); break; } d.presenter.OnWordSolved(); Rec("seq_enc_cheer", 36); Wait(0.3, 22); break;
                case 22: if (Recording()) { Wait(0.3, 22); break; } Shot("11_encourager_after_cheer"); Wait(0.3, 11); break;
                case 11:
                {
                    // Motion sequences for smoothness review: walking away outside, then idle front.
                    d.ExitReading();
                    d.walker.enabled = false; student.position = d.studentStart.position; d.walker.enabled = true;
                    Walk(new Vector2(0f, 1f));
                    Rec("seq_walk_away", 40); Wait(1.0, 12); break;
                }
                case 12: if (Recording()) { Wait(0.3, 12); break; } Walk(new Vector2(1f, 0f)); Rec("seq_walk_right", 40); Wait(1.0, 13); break;
                case 13: if (Recording()) { Wait(0.3, 13); break; } Walk(Vector2.zero); Wait(1.0, 14); break;
                case 14: Walk(new Vector2(0f, -0.001f)); Wait(0.1, 15); break;
                case 15: Walk(Vector2.zero); Rec("seq_idle", 120); Wait(1.0, 16); break;
                case 16: if (Recording()) { Wait(0.3, 16); break; } Wait(0.5, 17); break;
                default: stick.Simulated = null; Stop(); break;
            }
        }

        static void Stop()
        {
            SessionState.SetInt(StepKey, 99);
            EditorApplication.isPlaying = false;
        }
    }
}
