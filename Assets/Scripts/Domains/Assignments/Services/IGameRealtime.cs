using System;
using System.Threading.Tasks;
using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Services
{
    /// <summary>
    /// Instant delivery of game events within a session over a live connection
    /// (Supabase Realtime / WebSocket). It carries the SAME <see cref="GameEvent"/>s
    /// that are written to the database — the DB stays the source of truth and the
    /// poll remains a fallback; this just makes them arrive immediately. If the live
    /// connection can't be established, everything still works via polling.
    /// </summary>
    public interface IGameRealtime
    {
        /// <summary>Join a session's live channel; <paramref name="onEvent"/> fires on
        /// the main thread for events from OTHER players. Returns false if unavailable.</summary>
        Task<bool> JoinAsync(string sessionId, Action<GameEvent> onEvent);

        /// <summary>Leave a session's channel.</summary>
        void Leave(string sessionId);

        /// <summary>Push an event to everyone else in the session (best-effort).</summary>
        Task BroadcastAsync(string sessionId, GameEvent gameEvent);

        /// <summary>True while the socket is connected (else callers lean on polling).</summary>
        bool Connected { get; }
    }
}
