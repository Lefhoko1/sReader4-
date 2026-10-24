using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The "landing" page of the Reading House's own NavigationManager (the garden outside the
    /// door). The app's pages route here after "Sign out" (StudentHomeController / ProfilePageController
    /// → navigation.ShowLanding), so arriving here while signed in sends the student back outside.
    /// </summary>
    public sealed class HouseLandingPage : MonoBehaviour
    {
        public HouseDoor door;
        public ReadingHouseExploreDirector director;

        void OnEnable()
        {
            if (door != null && door.Unlocked && director != null) director.OnSignedOut();
        }
    }
}
