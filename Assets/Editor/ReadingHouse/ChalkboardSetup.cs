using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Game.ReadingHouse;
using SReader.UI.Views.Home;

namespace SReader.EditorTools.ReadingHouse
{
    /// <summary>
    /// Turns the chalkboard (SC_ReadingHouse_Integration, right wall) into a screen for the app's
    /// REAL pages: StudentHomeReal.uxml with its StudentHomeController, both unchanged — Home,
    /// Academies (browse / enrol), Tutors, Assignments and Friends, on live data. The house's
    /// AppCompositionRoot gets it in its existing studentHomeView slot, exactly as the app's home
    /// page does, and the page uses the house's NavigationManager. Re-running replaces only what
    /// this adds (and removes the earlier assignments card).
    /// </summary>
    public static class ChalkboardSetup
    {
        const string ScreenName = "Chalkboard Screen (app pages, reused)";
        const string StudentHomeUxml = "Assets/Scripts/UI/Views/Home/Student/StudentHomeReal.uxml";
        const string ProfileUxml = "Assets/Scripts/UI/Views/Profile/ProfileReal.uxml";

        [MenuItem("Tools/sReader/Reading House/Set Up Chalkboard Screen")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new System.InvalidOperationException("Stop Play mode first — the scene can only be changed in Edit mode.");
            var open = SceneManager.GetSceneByPath(ReadingHouseSceneBuilder.ScenePath);
            bool wasOpen = open.IsValid() && open.isLoaded;
            var scene = wasOpen ? open : EditorSceneManager.OpenScene(ReadingHouseSceneBuilder.ScenePath, OpenSceneMode.Additive);
            var previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);   // new objects go to the active scene (in batch mode that's an untitled one)
            try
            {
                var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
                T Find<T>() where T : Component => all.Where(t => t != null).Select(t => t.GetComponent<T>()).FirstOrDefault(c => c != null);

                var director = Find<ReadingHouseExploreDirector>() ?? throw new System.InvalidOperationException("No ReadingHouseExploreDirector in the scene.");
                var composition = Find<SReader.Core.DependencyInjection.AppCompositionRoot>() ?? throw new System.InvalidOperationException("No AppCompositionRoot in the house.");
                var hud = Find<ReadingHouseHud>();
                var slate = all.Select(t => t.GetComponent<Renderer>()).FirstOrDefault(r => r != null && r.name.StartsWith("SR_CHALKBOARD_Slate"))
                            ?? throw new System.InvalidOperationException("No chalkboard slate in the scene.");
                if (director.signInNavigation == null) throw new System.InvalidOperationException("The house has no NavigationManager (sign-in group).");

                // Earlier versions: the assignments card, and any previous screen.
                foreach (var t in all.Where(t => t != null && (t.name == "Assignments Board (app page, reused)" || t.name == ScreenName)).ToArray())
                    if (t != null) Object.DestroyImmediate(t.gameObject);

                var screenGo = new GameObject(ScreenName);
                if (director.transform.parent != null) screenGo.transform.SetParent(director.transform.parent, false);
                var screen = screenGo.AddComponent<ChalkboardScreen>();
                screen.slate = slate;
                screen.viewCamera = director.cameraRig.viewCamera;
                screen.chalkText = director.presenter.chalkboard != null ? director.presenter.chalkboard.text : null;
                screen.roomCentre = director.door.houseCentre;

                // The page: the app's student home, unchanged (inactive until the board opens).
                var pageGo = new GameObject("Student Home (StudentHomeReal.uxml)");
                pageGo.transform.SetParent(screenGo.transform, false);
                pageGo.SetActive(false);
                var doc = pageGo.AddComponent<UIDocument>();
                doc.panelSettings = hud.GetComponent<UIDocument>().panelSettings;   // the app's theme; the screen points a copy at its texture
                doc.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StudentHomeUxml);
                var home = pageGo.AddComponent<StudentHomeController>();
                Set(home, "navigation", director.signInNavigation);
                screen.page = doc;

                Set(composition, "studentHomeView", home);   // real ViewModels, the same way the app's home page gets them

                // The app's Profile page on the board too (the home's avatar button → navigation.ShowProfile).
                var profileGo = new GameObject("Profile (ProfileReal.uxml)");
                profileGo.transform.SetParent(screenGo.transform, false);
                profileGo.SetActive(false);
                var profileDoc = profileGo.AddComponent<UIDocument>();
                profileDoc.panelSettings = doc.panelSettings;
                profileDoc.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ProfileUxml);
                var profile = profileGo.AddComponent<SReader.UI.Views.ProfilePageController>();
                Set(profile, "navigation", director.signInNavigation);
                // A session restored straight onto Profile (profile not finished) still opens the doors.
                var unlock = profileGo.AddComponent<DoorUnlockPage>();
                unlock.door = director.door; unlock.director = director;
                screen.extraPages = new[] { profileDoc };
                Set(composition, "profileView", profile);
                Set(director.signInNavigation, "profilePage", profileGo);

                // Sign out (home or profile → navigation.ShowLanding) sends the student back outside.
                var landing = new SerializedObject(director.signInNavigation).FindProperty("landingPage").objectReferenceValue as GameObject;
                if (landing != null)
                {
                    var lp = landing.GetComponent<HouseLandingPage>() ?? landing.AddComponent<HouseLandingPage>();
                    lp.door = director.door; lp.director = director;
                    EditorUtility.SetDirty(lp);
                }

                director.chalkboardScreen = screen;
                director.chalkboard = slate.transform;
                director.studentSprite = Find<StudentSpriteAnimator>();
                EditorUtility.SetDirty(director);

                EditorSceneManager.SaveScene(scene);
                Debug.Log($"[Chalkboard] Screen on '{slate.name}' showing {StudentHomeUxml}; wired to the house's AppCompositionRoot.");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive.isLoaded && previousActive != scene) SceneManager.SetActiveScene(previousActive);
                if (!wasOpen) EditorSceneManager.CloseScene(scene, true);
            }
        }

        static void Set(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            var p = so.FindProperty(field) ?? throw new System.InvalidOperationException($"{target.GetType().Name} has no serialized field '{field}'.");
            p.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
