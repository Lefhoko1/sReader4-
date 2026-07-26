using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Repositories;

namespace SReader.Domains.Assignments.Services
{
    public sealed class AssignmentGameService : IAssignmentGameService
    {
        // A player who hasn't heart-beat within this window is treated as gone,
        // so the room can drop them and the turn can skip past them.
        public const double PresenceGraceSeconds = 15;

        readonly IAssignmentGameRepository repository;
        readonly CurrentSessionHolder session;
        readonly IClock clock;
        readonly IGameRealtime realtime;   // optional fast-path; null ⇒ poll only

        public AssignmentGameService(IAssignmentGameRepository repository, CurrentSessionHolder session, IClock clock,
            IGameRealtime realtime = null)
        {
            this.repository = Guard.NotNull(repository, nameof(repository));
            this.session    = Guard.NotNull(session, nameof(session));
            this.clock      = Guard.NotNull(clock, nameof(clock));
            this.realtime   = realtime;
        }

        // Push an appended event to everyone else instantly (best-effort).
        void Broadcast(GameEvent e)
        {
            if (realtime != null && e != null && !string.IsNullOrEmpty(e.SessionId))
                _ = realtime.BroadcastAsync(e.SessionId, e);
        }

        // ── Host / join ──────────────────────────────────────────────────────
        public async Task<Result<AssignmentGameSession>> HostAsync(Assignment assignment, string hostName, string avatarUrl, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Result.Fail<AssignmentGameSession>("Not signed in.");
            if (assignment == null || string.IsNullOrEmpty(assignment.Id))
                return Result.Fail<AssignmentGameSession>("An assignment is required.");

            var name = Clean(hostName);
            var created = await repository.CreateSessionAsync(new AssignmentGameSession
            {
                AssignmentId    = assignment.Id,
                ClassId         = assignment.ClassId,
                HostId          = session.CurrentUserId,
                HostName        = name,
                AssignmentTitle = assignment.Title,
                Code            = NewCode(),
                Status          = "open",
                CreatedAt       = clock.UtcNow
            }, ct);
            if (created.IsFailure) return created;

            // The host is seat 0 and a player too.
            var joined = await JoinInternalAsync(created.Value.Id, name, avatarUrl, ct);
            if (joined.IsFailure) return Result.Fail<AssignmentGameSession>(joined.Error);
            return created;
        }

        public Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId))
                return Task.FromResult(Result.Fail<IReadOnlyList<AssignmentGameSession>>("Assignment id is required."));
            return repository.ListOpenSessionsAsync(assignmentId, ct);
        }

        public async Task<Result<AssignmentGameSession>> JoinByCodeAsync(string code, string studentName, string avatarUrl, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Result.Fail<AssignmentGameSession>("Not signed in.");
            if (string.IsNullOrWhiteSpace(code)) return Result.Fail<AssignmentGameSession>("Enter a join code.");

            var found = await repository.GetByCodeAsync(code.Trim().ToUpperInvariant(), ct);
            if (found.IsFailure) return found;
            if (found.Value == null) return Result.Fail<AssignmentGameSession>("No session with that code.");

            var joined = await JoinInternalAsync(found.Value.Id, Clean(studentName), avatarUrl, ct);
            return joined.IsSuccess ? found : Result.Fail<AssignmentGameSession>(joined.Error);
        }

        public Task<Result> JoinSessionAsync(AssignmentGameSession gameSession, string studentName, string avatarUrl, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (gameSession == null || string.IsNullOrEmpty(gameSession.Id))
                return Task.FromResult(Result.Fail("A session is required."));
            return JoinInternalAsync(gameSession.Id, Clean(studentName), avatarUrl, ct);
        }

        // Joins (or re-joins) keeping a stable seat: an existing player keeps their
        // turn_order; a new player takes the next free seat.
        async Task<Result> JoinInternalAsync(string sessionId, string name, string avatarUrl, CancellationToken ct)
        {
            var existing = await repository.ListParticipantsAsync(sessionId, ct);
            int seat = 0;
            if (existing.IsSuccess)
            {
                var mine = existing.Value.FirstOrDefault(p => p.StudentId == session.CurrentUserId);
                seat = mine != null
                    ? mine.TurnOrder
                    : (existing.Value.Count == 0 ? 0 : existing.Value.Max(p => p.TurnOrder) + 1);
            }

            return await repository.JoinAsync(new GameParticipant
            {
                SessionId   = sessionId,
                StudentId   = session.CurrentUserId,
                StudentName = name,
                AvatarUrl   = avatarUrl,
                Score       = 0,
                Finished    = false,
                TurnOrder   = seat,
                LastSeen    = clock.UtcNow
            }, ct);
        }

        public Task<Result> LeaveSessionAsync(string sessionId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(sessionId)) return Task.FromResult(Result.Fail("Session id is required."));
            return repository.LeaveAsync(sessionId, session.CurrentUserId, ct);
        }

        // ── Presence ──────────────────────────────────────────────────────────
        public Task<Result> HeartbeatAsync(string sessionId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(sessionId)) return Task.FromResult(Result.Fail("Session id is required."));
            return repository.HeartbeatAsync(sessionId, session.CurrentUserId, ct);
        }

        public Task<Result> UpdateMyAvatarAsync(string sessionId, string avatarUrl, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(sessionId)) return Task.FromResult(Result.Fail("Session id is required."));
            return repository.UpdateAvatarAsync(sessionId, session.CurrentUserId, avatarUrl, ct);
        }

        public Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(sessionId))
                return Task.FromResult(Result.Fail<IReadOnlyList<GameParticipant>>("Session id is required."));
            return repository.ListParticipantsAsync(sessionId, ct);
        }

        public Task<Result<AssignmentGameSession>> RefreshSessionAsync(string sessionId, CancellationToken ct = default)
            => repository.GetSessionAsync(sessionId, ct);

        // ── Turn-based shared board ─────────────────────────────────────────
        public async Task<Result> StartGameAsync(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
        {
            if (gameSession == null) return Result.Fail("No session.");
            var first = ActiveOrdered(participants).FirstOrDefault();
            if (first == null) return Result.Fail("No active players to start.");

            // Carry the first player's id so receivers switch turn instantly.
            var started = await AppendAsync(gameSession.Id, GameEventType.Turn, -1, 0, first.StudentId, ct);
            if (started.IsSuccess) Broadcast(started.Value);
            return await repository.AdvanceTurnAsync(gameSession.Id, first.StudentId, gameSession.TurnIndex + 1, ct);
        }

        public async Task<Result> PassTurnAsync(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants, CancellationToken ct = default)
        {
            if (gameSession == null) return Result.Fail("No session.");
            var next = NextActiveAfter(participants, gameSession.CurrentTurnId);
            // The Turn event carries the next player's id (instant turn switch).
            var turn = await AppendAsync(gameSession.Id, GameEventType.Turn, -1, 0, next?.StudentId, ct);
            if (turn.IsSuccess) Broadcast(turn.Value);
            return await repository.AdvanceTurnAsync(gameSession.Id, next?.StudentId, gameSession.TurnIndex + 1, ct);
        }

        // Generic shared event (open / state / cancel / select-style) carrying an
        // optional small JSON payload — the stream that mirrors a player's moves.
        public async Task<Result<GameEvent>> ReportEventAsync(string sessionId, string type, int tokenIndex, string payload, CancellationToken ct = default)
        {
            var r = await AppendAsync(sessionId, type, tokenIndex, 0, payload, ct);
            if (r.IsSuccess) Broadcast(r.Value);
            return r;
        }

        public async Task<Result<GameEvent>> ReportSolveAsync(string sessionId, int tokenIndex, int points, int totalScore, bool finished, CancellationToken ct = default)
        {
            // My personal score is the source of truth for the scoreboard.
            await repository.UpdateScoreAsync(sessionId, session.CurrentUserId, totalScore, finished, ct);
            var r = await AppendAsync(sessionId, GameEventType.Solve, tokenIndex, points, null, ct);
            if (r.IsSuccess && r.Value != null) { r.Value.ActorScore = totalScore; Broadcast(r.Value); }
            return r;
        }

        public async Task<Result<GameEvent>> ReportFinishAsync(string sessionId, int totalScore, CancellationToken ct = default)
        {
            _ = repository.UpdateScoreAsync(sessionId, session.CurrentUserId, totalScore, true, ct);
            var r = await AppendAsync(sessionId, GameEventType.Finish, -1, totalScore, null, ct);
            if (r.IsSuccess && r.Value != null) { r.Value.ActorScore = totalScore; Broadcast(r.Value); }
            return r;
        }

        public Task<Result<IReadOnlyList<GameEvent>>> PullEventsAsync(string sessionId, long afterSeq, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(sessionId))
                return Task.FromResult(Result.Fail<IReadOnlyList<GameEvent>>("Session id is required."));
            return repository.ListEventsSinceAsync(sessionId, afterSeq, ct);
        }

        // Whether *this* client is the one responsible for auto-skipping a turn
        // that's stuck on a disconnected player (the lowest-seat active player,
        // so exactly one client acts — no thundering herd).
        public bool ShouldDriveAutoAdvance(AssignmentGameSession gameSession, IReadOnlyList<GameParticipant> participants)
        {
            if (gameSession == null || string.IsNullOrEmpty(gameSession.CurrentTurnId)) return false;
            var ordered = ActiveOrdered(participants);
            if (ordered.Count == 0) return false;

            var holder = participants?.FirstOrDefault(p => p.StudentId == gameSession.CurrentTurnId);
            var holderActive = holder != null && holder.IsActiveAt(clock.UtcNow, PresenceGraceSeconds);
            if (holderActive) return false;                          // turn-holder is fine

            return ordered[0].StudentId == session.CurrentUserId;    // I'm the leader → I advance
        }

        public bool IsMyTurn(AssignmentGameSession gameSession)
            => gameSession != null && gameSession.CurrentTurnId == session.CurrentUserId;

        // ── Helpers ────────────────────────────────────────────────────────
        Task<Result<GameEvent>> AppendAsync(string sessionId, string type, int tokenIndex, int points, string payload, CancellationToken ct)
            => repository.AppendEventAsync(new GameEvent
            {
                SessionId  = sessionId,
                ActorId    = session.CurrentUserId,
                ActorName  = null,
                Type       = type,
                TokenIndex = tokenIndex,
                Points     = points,
                Payload    = payload
            }, ct);

        List<GameParticipant> ActiveOrdered(IReadOnlyList<GameParticipant> participants)
        {
            var now = clock.UtcNow;
            return (participants ?? Array.Empty<GameParticipant>())
                .Where(p => p.IsActiveAt(now, PresenceGraceSeconds))
                .OrderBy(p => p.TurnOrder)
                .ToList();
        }

        // Walk the seat ring from the current holder to the next active player.
        GameParticipant NextActiveAfter(IReadOnlyList<GameParticipant> participants, string currentId)
        {
            var ordered = (participants ?? Array.Empty<GameParticipant>()).OrderBy(p => p.TurnOrder).ToList();
            if (ordered.Count == 0) return null;
            var now = clock.UtcNow;

            int start = 0;
            for (int i = 0; i < ordered.Count; i++)
                if (ordered[i].StudentId == currentId) { start = i; break; }

            for (int step = 1; step <= ordered.Count; step++)
            {
                var c = ordered[(start + step) % ordered.Count];
                if (c.IsActiveAt(now, PresenceGraceSeconds)) return c;
            }
            // No one else active — keep the current player if they're still here.
            var cur = ordered.FirstOrDefault(p => p.StudentId == currentId);
            return (cur != null && cur.IsActiveAt(now, PresenceGraceSeconds))
                ? cur
                : ordered.FirstOrDefault(p => p.IsActiveAt(now, PresenceGraceSeconds));
        }

        static string Clean(string name) => string.IsNullOrWhiteSpace(name) ? "A student" : name.Trim();

        // A short, unambiguous join code (no 0/O/1/I).
        static string NewCode()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new Random();
            var chars = new char[5];
            for (int i = 0; i < chars.Length; i++) chars[i] = alphabet[rng.Next(alphabet.Length)];
            return new string(chars);
        }
    }
}
