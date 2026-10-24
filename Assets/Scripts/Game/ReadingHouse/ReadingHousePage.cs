using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SReader.Domains.Assignments.Models;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// NavigationManager page for the Reading House (the 3D counterpart of
    /// TrekScenePage). While active it additively loads SC_ReadingHouse_Integration
    /// and gives its presenter the live assignment; when NavigationManager hides it
    /// (Back, Menu, or any other page) everything is unloaded and the app's
    /// portrait orientation is restored. Re-entrant (GEN-3): nothing is cached
    /// across deactivation.
    /// </summary>
    public sealed class ReadingHousePage : MonoBehaviour
    {
        NavigationManager navigation;
        AssignmentView view;
        StudentAssignmentsViewModel vm;

        Scene loaded;
        Scene previousActive;
        ScreenOrientation previousOrientation;
        bool orientationChanged;

        public void Configure(NavigationManager nav, AssignmentView assignment, StudentAssignmentsViewModel viewModel)
        {
            navigation = nav;
            view = assignment;
            vm = viewModel;
        }

        void OnEnable()
        {
            if (view == null) return;
            StartCoroutine(Open());
        }

        IEnumerator Open()
        {
            previousOrientation = Screen.orientation;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            orientationChanged = true;

            ReadingHousePresenter.SetPendingAssignment(view, vm, Exit);
            var op = SceneManager.LoadSceneAsync(ReadingHouseLauncher.SceneName, LoadSceneMode.Additive);
            yield return op;

            loaded = SceneManager.GetSceneByName(ReadingHouseLauncher.SceneName);
            if (!loaded.IsValid()) { Debug.LogError("[ReadingHouse] Scene failed to load."); Exit(); yield break; }

            // The Reading House's own lighting/ambient settings apply while it is the active scene.
            previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(loaded);
            SettleCamerasAndAudio();
        }

        /// <summary>Draw the Reading House above the app's camera, and keep one AudioListener.</summary>
        void SettleCamerasAndAudio()
        {
            float highest = float.MinValue;
            foreach (var cam in Camera.allCameras)
                if (cam.gameObject.scene != loaded) highest = Mathf.Max(highest, cam.depth);
            foreach (var root in loaded.GetRootGameObjects())
            {
                foreach (var cam in root.GetComponentsInChildren<Camera>(true))
                    if (highest > float.MinValue) cam.depth = highest + 1f;
                foreach (var listener in root.GetComponentsInChildren<AudioListener>(true))
                    listener.enabled = !OtherListenerExists();
            }
        }

        bool OtherListenerExists()
        {
            foreach (var l in FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
                if (l.enabled && l.gameObject.scene != loaded) return true;
            return false;
        }

        /// <summary>Leave the Reading House: back to the student home (the attempt stays as it was).</summary>
        public void Exit()
        {
            if (navigation != null) navigation.ShowStudentHome();
            else gameObject.SetActive(false);
        }

        void OnDisable()
        {
            StopAllCoroutines();
            ReadingHousePresenter.ClearPendingAssignment();
            if (loaded.IsValid() && loaded.isLoaded)
            {
                if (previousActive.IsValid() && previousActive.isLoaded) SceneManager.SetActiveScene(previousActive);
                SceneManager.UnloadSceneAsync(loaded);
            }
            loaded = default;
            if (orientationChanged)
            {
                Screen.orientation = previousOrientation;
                orientationChanged = false;
            }
        }
    }
}
