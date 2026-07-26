using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Repositories
{
    /// <summary>Multiplayer assignment-game sessions and their live participant scores.</summary>
    public interface IAssignmentGameRepository
    {
        Task<Result<AssignmentGameSession>> CreateSessionAsync(AssignmentGameSession session, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default);
        Task<Result<AssignmentGameSession>> GetByCodeAsync(string code, CancellationToken ct = default);
        Task<Result> UpdateSessionStatusAsync(string sessionId, string status, CancellationToken ct = default);

        Task<Result> JoinAsync(GameParticipant participant, CancellationToken ct = default);
        Task<Result> LeaveAsync(string sessionId, string studentId, CancellationToken ct = default);
        Task<Result> UpdateScoreAsync(string sessionId, string studentId, int score, bool finished, CancellationToken ct = default);
        Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default);

        // ── Presence ────────────────────────────────────────────────────────
        /// <summary>Refresh my last_seen so the room knows I'm still here.</summary>
        Task<Result> HeartbeatAsync(string sessionId, string studentId, CancellationToken ct = default);

        /// <summary>Set my participant avatar (only) — used to backfill a missing one.</summary>
        Task<Result> UpdateAvatarAsync(string sessionId, string studentId, string avatarUrl, CancellationToken ct = default);

        // ── Single session (for refreshing turn state) ──────────────────────
        Task<Result<AssignmentGameSession>> GetSessionAsync(string sessionId, CancellationToken ct = default);

        /// <summary>Set whose turn it is + bump the turn/state counters.</summary>
        Task<Result> AdvanceTurnAsync(string sessionId, string currentTurnId, int turnIndex, CancellationToken ct = default);

        // ── Shared event stream ─────────────────────────────────────────────
        Task<Result<GameEvent>> AppendEventAsync(GameEvent gameEvent, CancellationToken ct = default);
        Task<Result<IReadOnlyList<GameEvent>>> ListEventsSinceAsync(string sessionId, long afterSeq, CancellationToken ct = default);
    }
}
