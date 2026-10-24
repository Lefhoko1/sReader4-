using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SReader.Domains.Assignments.Models;
using SReader.UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SReader.Game.Classroom
{
    // Keeps the signed-in application and its services alive while the classroom is open.
    public static class ClassroomLaunch
    {
        public const string ScenePath = "Assets/Scenes/SC_LibraryRedesignPreview.unity";
        internal sealed class Context
        {
            public AssignmentView View;
            public StudentAssignmentsViewModel ViewModel;
            public Action Exit;
        }
        internal static Context Current { get; private set; }
        static readonly List<Action> restore = new List<Action>();
        static Scene previousScene;
        static bool loading;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { Current = null; loading = false; restore.Clear(); }

        public static bool TryOpen(AssignmentView view, StudentAssignmentsViewModel vm, Action onExit)
        {
            // Inside the Reading House the attempt happens at the dashboard: the student walks there.
            var house = UnityEngine.Object.FindAnyObjectByType<SReader.Game.ReadingHouse.ReadingHouseExploreDirector>();
            if (house != null && house.Active && view?.Assignment != null && !view.Submitted)
            {
                house.OnAttemptStarted(view, vm);
                return true;
            }
            if (loading || Current != null) return true;
            if (view?.Assignment == null || view.Submitted || !Application.CanStreamedLevelBeLoaded(ScenePath)) return false;
            Current = new Context { View = view, ViewModel = vm, Exit = onExit };
            Open();
            return true;
        }

        static async void Open()
        {
            loading = true;
            previousScene = SceneManager.GetActiveScene();
            try
            {
                foreach (var camera in UnityEngine.Object.FindObjectsByType<Camera>())
                    if (camera.enabled) { var c = camera; c.enabled = false; restore.Add(() => { if(c) c.enabled = true; }); }
                foreach (var listener in UnityEngine.Object.FindObjectsByType<AudioListener>())
                    if (listener.enabled) { var l = listener; l.enabled = false; restore.Add(() => { if(l) l.enabled = true; }); }
                foreach (var doc in UnityEngine.Object.FindObjectsByType<UIDocument>())
                {
                    var element = doc.rootVisualElement; var old = element.style.display;
                    element.style.display = DisplayStyle.None; restore.Add(() => { if(doc) element.style.display = old; });
                }
                foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>())
                    if (canvas.enabled) { var c = canvas; c.enabled = false; restore.Add(() => { if(c) c.enabled = true; }); }
                foreach (var events in UnityEngine.Object.FindObjectsByType<EventSystem>())
                    if (events.enabled) { var e = events; e.enabled = false; restore.Add(() => { if(e) e.enabled = true; }); }
                var operation = SceneManager.LoadSceneAsync(ScenePath, LoadSceneMode.Additive);
                if (operation == null) throw new InvalidOperationException("Classroom scene could not load.");
                while (!operation.isDone) await Task.Yield();
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(ScenePath));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                var callback = Current?.Exit; Restore(); callback?.Invoke();
            }
            finally { loading = false; }
        }

        internal static async void Close()
        {
            if (loading || Current == null) return;
            loading = true;
            var callback = Current.Exit;
            try
            {
                if (previousScene.IsValid() && previousScene.isLoaded) SceneManager.SetActiveScene(previousScene);
                var operation = SceneManager.UnloadSceneAsync(ScenePath);
                if (operation != null) while (!operation.isDone) await Task.Yield();
            }
            finally { Restore(); loading = false; callback?.Invoke(); }
        }

        static void Restore()
        {
            foreach (var action in restore) action();
            restore.Clear(); Current = null;
        }
    }
}
