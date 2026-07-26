using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Services
{
    /// <summary>
    /// Host / join / play a shared assignment-game session (multiplayer): live
    /// presence (avatars, disconnects), a turn-based shared board (the active
    /// player's moves appear on every screen) and a live scoreboard.
    /// </summary>
    public interface IAssignmentGameService
    {
        // Host / join (avatarUrl lets the room show who's playing).
        Task<Result<AssignmentGameSession>> HostAsync(Assignment assignment, string hostName, string avatarUrl, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default);
        Task<Result<AssignmentGameSession>> JoinByCodeAsync(string code, string studentName, string avatarUrl, CancellationToken ct = default);
        Task<Result> JoinSessionAsync(AssignmentGameSession session, string studentName, string avatarUrl, CancellationToken ct = default);
        Task<Result> LeaveSessionAsync(string sessionId, CancellationToken ct = default);

        // Presence.
        Task<Result> HeartbeatAsync(string sessionId, CancellationToken ct = default);
        Task<Result> UpdateMyAvatarAsync(string sessionId, string avatarUrl, CancellationToken ct = default);
        Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default);
        Task<Result<AssignmentGameSession>> RefreshSessionAsync(string sessionId, CancellationToken ct = default);

        // Turn-based shared board.
        Task<Result> StartGameAsync(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default);
        Task<Result> PassTurnAsync(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default);
        Task<Result<GameEvent>> ReportEventAsync(string sessionId, string type, int tokenIndex, string payload, CancellationToken ct = default);
        Task<Result<GameEvent>> ReportSolveAsync(string sessionId, int tokenIndex, int points, int totalScore, bool finished, CancellationToken ct = default);
        Task<Result<GameEvent>> ReportFinishAsync(string sessionId, int totalScore, CancellationToken ct = default);
        Task<Result<IReadOnlyList<GameEvent>>> PullEventsAsync(string sessionId, long afterSeq, CancellationToken ct = default);

        // Turn helpers.
        bool ShouldDriveAutoAdvance(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants);
        bool IsMyTurn(AssignmentGameSession session);
    }
}
