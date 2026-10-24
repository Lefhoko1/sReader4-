using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// The "home" page for the Reading House's own NavigationManager. The app's
    /// existing LoginPageController / RegisterPageController route here after a
    /// successful sign-in (through PostLoginRouter → ShowHomeForRole, or straight
    /// to the profile slot in placeholder mode). Arriving here unlocks the front
    /// doors — the student can then walk in.
    /// </summary>
    public sealed class DoorUnlockPage : MonoBehaviour
    {
        public HouseDoor door;
        public ReadingHouseExploreDirector director;

        void OnEnable()
        {
            bool wasLocked = door != null && !door.Unlocked;
            if (door != null) door.Unlock();
            if (director != null && wasLocked) director.OnSignedIn();
        }
    }
}
