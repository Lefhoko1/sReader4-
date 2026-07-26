using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;

namespace SReader.Infrastructure.Realtime
{
    /// <summary>
    /// Routes <see cref="IAssignmentGameService"/> calls to the backend the user has
    /// selected — the Supabase (database-backed) service or the Photon PUN
    /// (network-native, no DB) service. Switching is meant to happen in the lobby,
    /// before a session is live. If Photon is selected but not compiled in, falls
    /// back to Supabase.
    /// </summary>
    public sealed class SwitchableAssignmentGameService : IAssignmentGameService
    {
        readonly MultiplayerBackendSetting setting;
        readonly IAssignmentGameService supabase;
        readonly IAssignmentGameService photon;   // null when PUN isn't imported

        public SwitchableAssignmentGameService(MultiplayerBackendSetting setting,
            IAssignmentGameService supabase, IAssignmentGameService photon)
        {
            this.setting = setting;
            this.supabase = supabase;
            this.photon = photon;
        }

        IAssignmentGameService Active =>
            (setting != null && setting.UsingPhoton && photon != null) ? photon : supabase;

        public Task<Result<AssignmentGameSession>> HostAsync(Assignment assignment, string hostName, string avatarUrl, CancellationToken ct = default)
            => Active.HostAsync(assignment, hostName, avatarUrl, ct);

        public Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default)
            => Active.ListOpenSessionsAsync(assignmentId, ct);

        public Task<Result<AssignmentGameSession>> JoinByCodeAsync(string code, string studentName, string avatarUrl, CancellationToken ct = default)
            => Active.JoinByCodeAsync(code, studentName, avatarUrl, ct);

        public Task<Result> JoinSessionAsync(AssignmentGameSession session, string studentName, string avatarUrl, CancellationToken ct = default)
            => Active.JoinSessionAsync(session, studentName, avatarUrl, ct);

        public Task<Result> LeaveSessionAsync(string sessionId, CancellationToken ct = default)
            => Active.LeaveSessionAsync(sessionId, ct);

        public Task<Result> HeartbeatAsync(string sessionId, CancellationToken ct = default)
            => Active.HeartbeatAsync(sessionId, ct);

        public Task<Result> UpdateMyAvatarAsync(string sessionId, string avatarUrl, CancellationToken ct = default)
            => Active.UpdateMyAvatarAsync(sessionId, avatarUrl, ct);

        public Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default)
            => Active.ListParticipantsAsync(sessionId, ct);

        public Task<Result<AssignmentGameSession>> RefreshSessionAsync(string sessionId, CancellationToken ct = default)
            => Active.RefreshSessionAsync(sessionId, ct);

        public Task<Result> StartGameAsync(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
            => Active.StartGameAsync(session, participants, ct);

        public Task<Result> PassTurnAsync(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
            => Active.PassTurnAsync(session, participants, ct);

        public Task<Result<GameEvent>> ReportEventAsync(string sessionId, string type, int tokenIndex, string payload, CancellationToken ct = default)
            => Active.ReportEventAsync(sessionId, type, tokenIndex, payload, ct);

        public Task<Result<GameEvent>> ReportSolveAsync(string sessionId, int tokenIndex, int points, int totalScore, bool finished, CancellationToken ct = default)
            => Active.ReportSolveAsync(sessionId, tokenIndex, points, totalScore, finished, ct);

        public Task<Result<GameEvent>> ReportFinishAsync(string sessionId, int totalScore, CancellationToken ct = default)
            => Active.ReportFinishAsync(sessionId, totalScore, ct);

        public Task<Result<IReadOnlyList<GameEvent>>> PullEventsAsync(string sessionId, long afterSeq, CancellationToken ct = default)
            => Active.PullEventsAsync(sessionId, afterSeq, ct);

        public bool ShouldDriveAutoAdvance(AssignmentGameSession session, IReadOnlyList<GameParticipant> participants)
            => Active.ShouldDriveAutoAdvance(session, participants);

        public bool IsMyTurn(AssignmentGameSession session) => Active.IsMyTurn(session);
    }
}
