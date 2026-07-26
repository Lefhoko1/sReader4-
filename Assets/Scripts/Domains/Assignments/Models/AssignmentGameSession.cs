using System;
using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// A shared play session for one assignment: students join (by code or from
    /// the open-sessions list among classmates) and play the same content while a
    /// live scoreboard tracks everyone. Backed by Supabase (polled), so it works
    /// without realtime netcode — and a student can always just play solo with no
    /// session at all.
    /// </summary>
    public class AssignmentGameSession : Entity
    {
        public string AssignmentId { get; set; }
        public string ClassId { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string AssignmentTitle { get; set; }

        /// <summary>Short human join code (e.g. "7KQ2").</summary>
        public string Code { get; set; }

        /// <summary>"open" (lobby) | "playing" | "finished".</summary>
        public string Status { get; set; } = "open";

        public DateTime CreatedAt { get; set; }

        public bool IsOpen => Status == "open";
        public bool IsPlaying => Status == "playing";

        // ── Shared turn-based board state ───────────────────────────────────
        /// <summary>The student whose turn it is to act on the shared board.</summary>
        public string CurrentTurnId { get; set; }

        /// <summary>Monotonic turn counter (also drives "next player" rotation).</summary>
        public int TurnIndex { get; set; }

        /// <summary>Bumped on every shared-state change; lets clients detect drift.</summary>
        public int StateVersion { get; set; }

        public DateTime? StartedAt { get; set; }
    }

    /// <summary>One player's live row in a session — name, avatar, score, presence.</summary>
    public class GameParticipant : Entity
    {
        public string SessionId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string AvatarUrl { get; set; }
        public int Score { get; set; }
        public bool Finished { get; set; }

        /// <summary>Stable seat order for turn rotation (assigned at join).</summary>
        public int TurnOrder { get; set; }

        /// <summary>Last heartbeat. A player who stops heart-beating is treated as gone.</summary>
        public DateTime LastSeen { get; set; }

        public DateTime UpdatedAt { get; set; }

        /// <summary>Present = heart-beat seen within the grace window (default 15s).</summary>
        public bool IsActiveAt(DateTime nowUtc, double graceSeconds = 15)
            => (nowUtc - LastSeen).TotalSeconds <= graceSeconds;
    }

    /// <summary>
    /// One entry in a session's shared event stream — the "broadcast" log that
    /// makes every screen converge on the same board. A player appends an event
    /// (selected a token, solved it, passed the turn, finished) and the others
    /// read events after their last cursor and apply them.
    /// </summary>
    public class GameEvent : Entity
    {
        public long Seq { get; set; }
        public string SessionId { get; set; }
        public string ActorId { get; set; }
        public string ActorName { get; set; }

        /// <summary>One of <see cref="GameEventType"/>.</summary>
        public string Type { get; set; }

        /// <summary>Which board activity this is about (deterministic order), or -1.</summary>
        public int TokenIndex { get; set; } = -1;

        public int Points { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Transient (not persisted): the actor's total score, carried on a
        /// realtime Solve/Finish broadcast so scoreboards update instantly.</summary>
        public int ActorScore { get; set; }
    }

    /// <summary>The kinds of shared events on the board.</summary>
    public static class GameEventType
    {
        public const string Open   = "open";     // active player opened a token's mini-game → modal opens on every screen
        public const string State  = "state";    // live snapshot of the open mini-game (arrangement / error / hint)
        public const string Solve  = "solve";    // a token was solved (mark done everywhere + award points) → success effect
        public const string Cancel = "cancel";   // active player closed the mini-game without solving → modal closes everywhere
        public const string Turn   = "turn";     // the turn passed to the next player
        public const string Start  = "start";    // host started the game
        public const string Finish = "finish";   // a player finished the whole board

        // ── Collaborative / social (transient; no board-state change) ────────
        public const string Cheer      = "cheer";      // an emoji reaction, floats on every screen
        public const string Tip        = "tip";        // a solver's "how I knew it" note, shown to peers
        public const string AlsoSolved = "alsosolved"; // a spectator solved the same word in practice
    }
}
