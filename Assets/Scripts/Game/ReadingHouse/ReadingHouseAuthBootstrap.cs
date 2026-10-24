using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Switches on the house's sign-in group (the app's AppCompositionRoot,
    /// NavigationManager and Login/Register pages, reused unchanged) only when the
    /// Reading House runs on its own. When the app opens the house for an
    /// assignment the student is already signed in, so the group stays off (no
    /// second composition root) and the doors start unlocked.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public sealed class ReadingHouseAuthBootstrap : MonoBehaviour
    {
        public GameObject signInGroup;
        public HouseDoor door;

        void Awake()
        {
            if (ReadingHousePresenter.IsLaunchPending)
            {
                if (signInGroup != null) signInGroup.SetActive(false);
                if (door != null) door.Unlock();
                return;
            }
            if (signInGroup != null) signInGroup.SetActive(true);
        }
    }
}
