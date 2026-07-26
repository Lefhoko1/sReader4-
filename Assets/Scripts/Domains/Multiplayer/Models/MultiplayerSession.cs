using System;
using SReader.Core.Common;

namespace SReader.Domains.Multiplayer.Models
{
    /// <summary>Session metadata persisted for discovery/history — live state lives in the network layer.</summary>
    public class MultiplayerSession : Entity
    {
        public string HostUserId { get; set; }
        public string SessionName { get; set; }
        public int MaxPlayers { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
