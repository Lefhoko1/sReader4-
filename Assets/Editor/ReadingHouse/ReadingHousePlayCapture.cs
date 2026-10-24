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
    /// Plays SC_ReadingHouse_Integration (only this scene) and screenshots the
    /// walk-in flow from the Game view — including the UI Toolkit sign-in card,
    /// which camera renders cannot show. Stops Play mode afterwards and reopens
    /// the scene that was open. Nothing is saved.
    /// </summary>
    [InitializeOnLoad]
    public static class ReadingHousePlayCapture
    {
        const string StateKey = "sReader.RH.PlayCapture.Step";
        const string ReturnKey = "sReader.RH.PlayCapture.Return";
        const string BgKey = "sReader.RH.PlayCapture.Bg";
        static readonly string Out = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GeneratedAssets", "ReadingHouseUnity", "playcapture"));
        static double next;

        static ReadingHousePlayCapture()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += s =>
            {
                if (s == PlayModeStateChange.EnteredEditMode && SessionState.GetInt(StateKey, -1) == 99)
                {
                    SessionState.EraseInt(StateKey);
                    PlayerSettings.runInBackground = SessionState.GetBool(BgKey, false);
                    var back = SessionState.GetString(ReturnKey, "");
                    if (!string.IsNullOrEmpty(back) && File.Exists(back)) EditorSceneManager.OpenScene(back, OpenSceneMode.Single);
                }
                if (s == PlayModeStateChange.EnteredPlayMode && SessionState.GetInt(StateKey, -1) == 0)
                    next = EditorApplication.timeSinceStartup + 2.5;
            };
        }

        public static void Start()
        {
            Directory.CreateDirectory(Out);
            foreach (var f in Directory.GetFiles(Out, "*.png")) File.Delete(f);
            SessionState.SetString(ReturnKey, SceneManager.GetActiveScene().path);
            SessionState.SetBool(BgKey, PlayerSettings.runInBackground);
            PlayerSettings.runInBackground = true;
            EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Single);
            SessionState.SetInt(StateKey, 0);
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            int step = SessionState.GetInt(StateKey, -1);
            if (step < 0 || step == 99 || !EditorApplication.isPlaying) return;
            // The Game view is not repainted while the editor is unfocused; keep it live so screenshots land.
            RepaintGameView();
            if (EditorApplication.timeSinceStartup < next) return;
            try { Step(step); }
            catch (Exception e) { File.WriteAllText(Path.Combine(Out, "ERROR.txt"), e.ToString()); Stop(); }
        }

        static EditorWindow gameView;
        static double lastRepaint;

        /// <summary>Repaint just the Game view (~30 times a second); repainting every editor window stalls play mode.</summary>
        static void RepaintGameView()
        {
            if (EditorApplication.timeSinceStartup - lastRepaint < 0.033) return;
            lastRepaint = EditorApplication.timeSinceStartup;
            if (gameView == null)
            {
                var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
                var all = Resources.FindObjectsOfTypeAll(type);
                gameView = all.Length > 0 ? (EditorWindow)all[0] : null;
            }
            if (gameView != null) gameView.Repaint();
        }

        static void Shot(string name) => ScreenCapture.CaptureScreenshot(Path.Combine(Out, name));

        static void Step(int step)
        {
            var d = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            var student = d.walker.transform;
            var outward = Vector3.ProjectOnPlane(d.door.transform.position - d.door.houseCentre.position, Vector3.up).normalized;
            void Wait(double s, int nextStep) { next = EditorApplication.timeSinceStartup + s; SessionState.SetInt(StateKey, nextStep); }
            void Place(Vector3 p) { d.walker.enabled = false; student.position = p; d.walker.enabled = true; }

            switch (step)
            {
                case 0: Shot("01_opening.png"); Wait(1.0, 1); break;
                case 1: Place(d.door.transform.position + outward * 2.2f - Vector3.Cross(Vector3.up, -outward) * 0.6f); d.follow.Follow(); Wait(2.0, 2); break;
                case 2: Shot("02_walked_to_door_follow.png"); Wait(1.0, 3); break;
                case 3:
                {
                    // Tap the Log in panel through the real input path (what a finger does).
                    var panel = Object.FindObjectsByType<DoorPanelTarget>(FindObjectsInactive.Exclude).First(p => p.kind == DoorPanelTarget.Kind.Login);
                    var cam = d.cameraRig.viewCamera;
                    var screen = cam.WorldToScreenPoint(panel.GetComponent<Renderer>().bounds.center);
                    Object.FindAnyObjectByType<ReadingHouseInput>().Tap(screen);
                    File.WriteAllText(Path.Combine(Out, "TAP_RESULT.txt"), d.OverlayOpen ? "Log in panel tap opened the card." : "Log in panel tap did NOT open the card.");
                    if (!d.OverlayOpen) d.signInNavigation.ShowLogin();
                    Wait(2.6, 4);
                    break;
                }
                case 4: Shot("03_login_card.png"); Wait(1.0, 5); break;
                case 5: d.signInNavigation.ShowRegister(); Wait(1.6, 6); break;
                case 6: Shot("04_register_card.png"); Wait(1.0, 7); break;
                case 7: d.signInNavigation.ShowLanding(); d.door.Unlock(); Wait(1.8, 8); break;
                case 8: Shot("05_card_closed_doors_open.png"); Wait(1.0, 9); break;
                case 9: Place(d.presenter.studentMover.overviewSpot.position + outward * 0.2f); Wait(2.2, 10); break;
                case 10: Shot("06_inside_cutaway.png"); Wait(1.0, 11); break;
                case 11: Place(student.position - Vector3.Cross(Vector3.up, -outward) * 2.6f - outward * 0.6f); Wait(2.0, 12); break;
                case 12: Shot("07_inside_by_bookshelves.png"); Wait(1.2, 13); break;
                default: Stop(); break;
            }
        }

        static void Stop()
        {
            SessionState.SetInt(StateKey, 99);
            EditorApplication.isPlaying = false;
        }
    }
}
