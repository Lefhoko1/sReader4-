using UnityEngine;

namespace SReader.Game.ReadingHouse
{
    /// <summary>Which sign-in panel beside the front door a collider belongs to. No behaviour of its own.</summary>
    public sealed class DoorPanelTarget : MonoBehaviour
    {
        public enum Kind { Login, Register }
        public Kind kind;
    }
}
