using UnityEngine;
using SReader.Domains.Assignments.Models;
using SReader.UI.Navigation;
using SReader.UI.ViewModels;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Opens the 3D Reading House for one assignment from the app, the same way
    /// "▶ Story Trek" opens the Trek: the EXISTING AssignmentView and
    /// StudentAssignmentsViewModel are handed over, and NavigationManager keeps
    /// owning the screen (one page at a time). The page wrapper is created in code,
    /// so no existing scene has to be edited to add it.
    /// </summary>
    public static class ReadingHouseLauncher
    {
        public const string SceneName = "SC_ReadingHouse_Integration";

        static ReadingHousePage page;

        /// <summary>True when the Reading House scene is in the build (otherwise the launch button stays hidden).</summary>
        public static bool IsAvailable => Application.CanStreamedLevelBeLoaded(SceneName);

        public static void Launch(AssignmentView view, StudentAssignmentsViewModel vm)
        {
            // Already inside the house (the app's pages on the chalkboard): read it here, on the dashboard.
            var house = Object.FindAnyObjectByType<ReadingHouseExploreDirector>();
            if (house != null && house.Active)
            {
                house.OnAssignmentChosen(view, vm);
                return;
            }

            var navigation = Object.FindAnyObjectByType<NavigationManager>();
            if (navigation == null)
            {
                Debug.LogError("[ReadingHouse] No NavigationManager in the scene — cannot open the Reading House.");
                return;
            }
            if (page == null)
            {
                var go = new GameObject("ReadingHousePage (runtime)");
                go.SetActive(false);                      // configure before OnEnable runs
                page = go.AddComponent<ReadingHousePage>();
            }
            page.Configure(navigation, view, vm);
            navigation.ShowRuntimePage(page.gameObject, "Reading House");
        }
    }
}
