using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>
    /// Tag on a dashboard collider so a tap can be routed to the right control.
    /// Own file so Unity can serialise it (one MonoBehaviour per file).
    /// </summary>
    public sealed class DashboardControlTarget : MonoBehaviour
    {
        public DashboardControl control;
    }
}
