using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SReader.Domains.Assignments.Models;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.Game.Trek
{
    /// <summary>
    /// NavigationManager page wrapper for the Story Trek (spec §3). NavigationManager
    /// activates/deactivates this GameObject like any other page (GEN-1/2); while
    /// active, a <see cref="TrekPresenter"/> runs the game — the 2D world is built
    /// procedurally under this page, and an optional authored scene can still be
    /// additive-loaded behind it. Everything is torn down on hide so screens never
    /// leak.
    ///
    /// Re-entrancy (GEN-3): the presenter and overlay are rebuilt in OnEnable on
    /// every activation; nothing visual is cached across deactivation.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class TrekScenePage : MonoBehaviour
    {
        [SerializeField] private NavigationManager navigation;

        [Tooltip("Optional: an authored Trek scene to load additively behind the page. " +
                 "Leave empty — Phase 1 builds its world procedurally.")]
        [SerializeField] private string trekSceneName = "";

        // The pending launch payload, set by Launch(...) just before the page is
        // shown (static: the launcher holds no reference to this inactive page).
        static AssignmentView pendingView;
        static StudentAssignmentsViewModel pendingVm;
        static Action pendingExit;

        VisualElement root;
        TrekPresenter presenter;
        bool sceneLoaded;

        /// <summary>
        /// Open the Trek for an assignment. <paramref name="view"/>/<paramref name="vm"/>
        /// may be null — the trek then runs the bundled practice passage (GEN-4).
        /// <paramref name="onExit"/> is where Back returns (defaults to Student Home).
        /// </summary>
        public void Launch(AssignmentView view = null, StudentAssignmentsViewModel vm = null, Action onExit = null)
        {
            pendingView = view;
            pendingVm = vm;
            pendingExit = onExit;
            if (navigation != null) navigation.ShowTrek();
            else Debug.LogError($"[{name}] Navigation is not assigned — cannot open the Trek.", this);
        }

        void OnEnable()
        {
            if (navigation == null)
            {
                Debug.LogError($"[{name}] Navigation is not assigned in the Inspector — the Trek cannot exit.", this);
                return;
            }

            root = GetComponent<UIDocument>().rootVisualElement;
            root.RegisterCallback<NavigationCancelEvent>(OnSystemBack);

            LoadTrekSceneIfAny();

            presenter = GetComponent<TrekPresenter>();
            if (presenter == null) presenter = gameObject.AddComponent<TrekPresenter>();
            presenter.Begin(root, pendingView, pendingVm, ExitTarget());
        }

        void OnDisable()
        {
            if (root != null)
            {
                root.UnregisterCallback<NavigationCancelEvent>(OnSystemBack);
                root.Clear();
            }
            UnloadTrekScene();
        }

        Action ExitTarget()
        {
            var exit = pendingExit;
            pendingView = null;
            pendingVm = null;
            pendingExit = null;
            return exit ?? (() => navigation.ShowStudentHome());
        }

        // System back / Escape: save-and-exit through the presenter so the attempt
        // stays in progress at the checkpoint (R-9).
        void OnSystemBack(NavigationCancelEvent evt)
        {
            evt.StopPropagation();
            if (presenter != null) presenter.RequestExit();
            else navigation.ShowStudentHome();
        }

        // ── optional authored scene (unused in Phase 1) ──────────────────────

        void LoadTrekSceneIfAny()
        {
            if (string.IsNullOrEmpty(trekSceneName) || sceneLoaded) return;
            if (!Application.CanStreamedLevelBeLoaded(trekSceneName))
            {
                Debug.LogWarning($"[{name}] Trek scene '{trekSceneName}' is not in Build Settings; running procedurally.", this);
                return;
            }
            SceneManager.LoadSceneAsync(trekSceneName, LoadSceneMode.Additive);
            sceneLoaded = true;
        }

        void UnloadTrekScene()
        {
            if (!sceneLoaded) return;
            var scene = SceneManager.GetSceneByName(trekSceneName);
            if (scene.IsValid() && scene.isLoaded)
                SceneManager.UnloadSceneAsync(scene);
            sceneLoaded = false;
        }
    }
}
