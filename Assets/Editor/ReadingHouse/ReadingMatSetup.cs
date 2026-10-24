using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Game.ReadingHouse;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Reading at the dashboard as a game (SC_ReadingHouse_Integration):
    ///   • the sentence as Word Path stones (SM_WordStone_A/B/C/Key + WordStone) on the rug in front of
    ///     the student — MatStoneBoard; the dashboard tray's stones are no longer used;
    ///   • the dashboard's display as a game screen for the word activities, the reader and the reading
    ///     toolbar — DashboardScreen;
    ///   • the reading camera raised and drawn back so it holds the display, the rug of stones, the student
    ///     and both helpers on their chairs.
    /// Re-running replaces only what this adds.
    /// </summary>
    public static class ReadingMatSetup
    {
        static readonly string[] StonePrefabs = { "Assets/Prefabs/SM_WordStone_A.prefab", "Assets/Prefabs/SM_WordStone_B.prefab", "Assets/Prefabs/SM_WordStone_C.prefab" };
        const string KeyPrefab = "Assets/Prefabs/SM_WordStone_Key.prefab";
        static readonly Vector3 GameplayCameraAt = new Vector3(0f, 3.7f, 5.2f), GameplayCameraAim = new Vector3(0f, 1.7f, -1.0f);
        const float GameplayFov = 56f;

        [MenuItem("Tools/sReader/Reading House/Set Up Reading Mat + Dashboard Screen")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first — the scene can only be changed in Edit mode.");
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            try
            {
                foreach (var old in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true))
                             .Where(t => t != null && (t.GetComponent<MatStoneBoard>() != null || t.GetComponent<DashboardScreen>() != null))
                             .Select(t => t.gameObject).ToArray())
                    Object.DestroyImmediate(old);
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                var presenter = all.Select(t => t.GetComponent<ReadingHousePresenter>()).First(p => p != null);
                var director = all.Select(t => t.GetComponent<ReadingHouseExploreDirector>()).FirstOrDefault(d => d != null);
                var hud = all.Select(t => t.GetComponent<ReadingHouseHud>()).FirstOrDefault(h => h != null);
                Renderer R(string name) => all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name == name)
                                           ?? throw new System.InvalidOperationException($"{name} not found.");
                var rug = R("SR_Rug");
                var display = R("SR_DISPLAY_MAIN");
                var cam = presenter.cameraRig.viewCamera;

                // The stones on the rug.
                var matGo = new GameObject("Reading Mat Stones (Word Path)");
                var mat = matGo.AddComponent<MatStoneBoard>();
                mat.stonePrefabs = StonePrefabs.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(p => p != null).ToArray();
                mat.keyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(KeyPrefab);
                if (mat.stonePrefabs.Length == 0 || mat.keyPrefab == null) throw new System.InvalidOperationException("Word Path stone prefabs not found in Assets/Prefabs.");
                mat.mat = rug;
                mat.student = presenter.studentMover.gameplaySpot;
                mat.viewCamera = cam;
                mat.dashboard = display.transform;
                presenter.matStones = mat;

                // The dashboard's display as a game screen (under the dashboard, so taps on it count as the dashboard).
                var screenGo = new GameObject("Dashboard Screen (activities, reader, toolbar)");
                screenGo.transform.SetParent(presenter.dashboard.transform.root, true);
                var screen = screenGo.AddComponent<DashboardScreen>();
                screen.slate = display;
                screen.viewCamera = cam;
                screen.transparent = true;
                screen.layoutWidth = 1150f;
                screen.fill = 0.98f;
                screen.roomCentre = director != null ? director.door.houseCentre : null;
                var pageGo = new GameObject("Dashboard Page (UI Toolkit)");
                pageGo.transform.SetParent(screenGo.transform, false);
                pageGo.SetActive(false);
                var doc = pageGo.AddComponent<UIDocument>();
                doc.panelSettings = hud != null ? hud.GetComponent<UIDocument>().panelSettings : null;
                screen.page = doc;
                presenter.dashboardScreen = screen;

                // The reading camera: back and up, so the display, the stones, the student and both helpers are in view.
                var pose = presenter.cameraRig.poses.First(p => p != null && p.view == ReadingHouseView.Gameplay);
                pose.position.position = GameplayCameraAt;
                pose.aim.position = GameplayCameraAim;
                pose.referenceVerticalFov = GameplayFov;
                // A close view of the display for the activities and the reader (the rig's third view).
                var focus = presenter.cameraRig.poses.First(p => p != null && p.view == ReadingHouseView.TrayFocus);
                focus.position.position = new Vector3(0f, 2.75f, 2.7f);
                focus.aim.position = display.bounds.center;
                focus.referenceVerticalFov = 42f;
                EditorUtility.SetDirty(focus.position); EditorUtility.SetDirty(focus.aim);
                EditorUtility.SetDirty(pose.position); EditorUtility.SetDirty(pose.aim);
                EditorUtility.SetDirty(presenter.cameraRig);
                EditorUtility.SetDirty(presenter);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[ReadingMat] Stones on '{rug.name}', dashboard screen on '{display.name}', reading camera at {GameplayCameraAt}.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive.isLoaded && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
