using System;
using UnityEngine;

namespace SReader.Domains.Multiplayer.Models
{
    /// <summary>Last-known persistence snapshot (where to respawn) — not live state.</summary>
    public class PlayerState
    {
        public string UserId { get; set; }

        public string LastScene { get; set; }
        public Vector3 LastPosition { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
