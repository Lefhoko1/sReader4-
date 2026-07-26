using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// PostgREST-backed multiplayer game sessions over {SupabaseUrl}/rest/v1.
    /// Tables: assignment_game_sessions, assignment_game_participants. Sessions
    /// are broadly readable to signed-in users (they're just lobbies); only the
    /// host writes a session and only a player writes their own participant row.
    /// </summary>
    public sealed class SupabaseAssignmentGameRepository : SupabaseRepositoryBase, IAssignmentGameRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseAssignmentGameRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<AssignmentGameSession>> CreateSessionAsync(AssignmentGameSession s, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AssignmentGameSession>(configured.Error);

            var body = JsonUtility.ToJson(SessionWriteDto.From(s));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/assignment_game_sessions",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<AssignmentGameSession>(response.Error);

            var rows = ParseArray<SessionDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AssignmentGameSession>("Session created but not returned.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<IReadOnlyList<AssignmentGameSession>>> ListOpenSessionsAsync(string assignmentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AssignmentGameSession>>(configured.Error);

            var url = $"{RestUrl}/assignment_game_sessions?assignment_id=eq.{Uri.EscapeDataString(assignmentId)}&status=eq.open&select=*&order=created_at.desc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AssignmentGameSession>>(response.Error);

            var rows = ParseArray<SessionDto>(response.Value);
            var list = new List<AssignmentGameSession>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AssignmentGameSession>>(list);
        }

        public async Task<Result<AssignmentGameSession>> GetByCodeAsync(string code, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AssignmentGameSession>(configured.Error);

            var url = $"{RestUrl}/assignment_game_sessions?code=eq.{Uri.EscapeDataString(code)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AssignmentGameSession>(response.Error);

            var rows = ParseArray<SessionDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public async Task<Result> UpdateSessionStatusAsync(string sessionId, string status, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new StatusPatch { status = status });
            var url = $"{RestUrl}/assignment_game_sessions?id=eq.{Uri.EscapeDataString(sessionId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> JoinAsync(GameParticipant p, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(ParticipantWriteDto.From(p));
            var url = $"{RestUrl}/assignment_game_participants?on_conflict=session_id,student_id";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, url,
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> LeaveAsync(string sessionId, string studentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/assignment_game_participants?session_id=eq.{Uri.EscapeDataString(sessionId)}&student_id=eq.{Uri.EscapeDataString(studentId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UpdateScoreAsync(string sessionId, string studentId, int score, bool finished, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = "{\"score\":" + score + ",\"finished\":" + (finished ? "true" : "false")
                + ",\"updated_at\":\"" + DateTime.UtcNow.ToString("o") + "\"}";
            var url = $"{RestUrl}/assignment_game_participants?session_id=eq.{Uri.EscapeDataString(sessionId)}&student_id=eq.{Uri.EscapeDataString(studentId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<IReadOnlyList<GameParticipant>>> ListParticipantsAsync(string sessionId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<GameParticipant>>(configured.Error);

            var url = $"{RestUrl}/assignment_game_participants?session_id=eq.{Uri.EscapeDataString(sessionId)}&select=*&order=turn_order.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<GameParticipant>>(response.Error);

            var rows = ParseArray<ParticipantDto>(response.Value);
            var list = new List<GameParticipant>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<GameParticipant>>(list);
        }

        // ── Presence ────────────────────────────────────────────────────────
        public async Task<Result> HeartbeatAsync(string sessionId, string studentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = "{\"last_seen\":\"" + DateTime.UtcNow.ToString("o") + "\"}";
            var url = $"{RestUrl}/assignment_game_participants?session_id=eq.{Uri.EscapeDataString(sessionId)}&student_id=eq.{Uri.EscapeDataString(studentId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UpdateAvatarAsync(string sessionId, string studentId, string avatarUrl, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var json = string.IsNullOrEmpty(avatarUrl)
                ? "null"
                : "\"" + avatarUrl.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            var body = "{\"avatar_url\":" + json + "}";
            var url = $"{RestUrl}/assignment_game_participants?session_id=eq.{Uri.EscapeDataString(sessionId)}&student_id=eq.{Uri.EscapeDataString(studentId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Single session / turn state ─────────────────────────────────────
        public async Task<Result<AssignmentGameSession>> GetSessionAsync(string sessionId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AssignmentGameSession>(configured.Error);

            var url = $"{RestUrl}/assignment_game_sessions?id=eq.{Uri.EscapeDataString(sessionId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AssignmentGameSession>(response.Error);

            var rows = ParseArray<SessionDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public async Task<Result> AdvanceTurnAsync(string sessionId, string currentTurnId, int turnIndex, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            // Hand-built so the nullable uuid emits real JSON null when no player
            // can take the turn (JsonUtility would write "" and Postgres rejects it).
            var turnJson = string.IsNullOrEmpty(currentTurnId) ? "null" : "\"" + currentTurnId + "\"";
            var body = "{\"current_turn_id\":" + turnJson
                + ",\"turn_index\":" + turnIndex
                + ",\"state_version\":" + turnIndex
                + ",\"status\":\"playing\""
                + ",\"started_at\":\"" + DateTime.UtcNow.ToString("o") + "\"}";
            var url = $"{RestUrl}/assignment_game_sessions?id=eq.{Uri.EscapeDataString(sessionId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Shared event stream ─────────────────────────────────────────────
        public async Task<Result<GameEvent>> AppendEventAsync(GameEvent e, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<GameEvent>(configured.Error);

            var body = JsonUtility.ToJson(EventWriteDto.From(e));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/assignment_game_events",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<GameEvent>(response.Error);

            var rows = ParseArray<EventDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public async Task<Result<IReadOnlyList<GameEvent>>> ListEventsSinceAsync(string sessionId, long afterSeq, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<GameEvent>>(configured.Error);

            var url = $"{RestUrl}/assignment_game_events?session_id=eq.{Uri.EscapeDataString(sessionId)}&seq=gt.{afterSeq}&select=*&order=seq.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<GameEvent>>(response.Error);

            var rows = ParseArray<EventDto>(response.Value);
            var list = new List<GameEvent>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<GameEvent>>(list);
        }

        // ── DTOs ──────────────────────────────────────────────────────────────

        [Serializable]
        class SessionDto
        {
            public string id;
            public string assignment_id;
            public string class_id;
            public string host_id;
            public string host_name;
            public string assignment_title;
            public string code;
            public string status;
            public string created_at;
            public string current_turn_id;
            public int turn_index;
            public int state_version;
            public string started_at;

            public AssignmentGameSession ToModel() => new AssignmentGameSession
            {
                Id = id, AssignmentId = assignment_id, ClassId = class_id, HostId = host_id,
                HostName = host_name, AssignmentTitle = assignment_title, Code = code,
                Status = status, CreatedAt = ParseDate(created_at),
                CurrentTurnId = current_turn_id, TurnIndex = turn_index, StateVersion = state_version,
                StartedAt = string.IsNullOrEmpty(started_at) ? (DateTime?)null : ParseDate(started_at)
            };
        }

        [Serializable]
        class SessionWriteDto
        {
            public string assignment_id;
            public string class_id;
            public string host_id;
            public string host_name;
            public string assignment_title;
            public string code;
            public string status;

            public static SessionWriteDto From(AssignmentGameSession s) => new SessionWriteDto
            {
                assignment_id = s.AssignmentId,
                class_id = string.IsNullOrEmpty(s.ClassId) ? null : s.ClassId,
                host_id = s.HostId,
                host_name = s.HostName,
                assignment_title = s.AssignmentTitle,
                code = s.Code,
                status = string.IsNullOrEmpty(s.Status) ? "open" : s.Status
            };
        }

        [Serializable]
        class ParticipantDto
        {
            public string id;
            public string session_id;
            public string student_id;
            public string student_name;
            public string avatar_url;
            public int score;
            public bool finished;
            public int turn_order;
            public string last_seen;
            public string updated_at;

            public GameParticipant ToModel() => new GameParticipant
            {
                Id = id, SessionId = session_id, StudentId = student_id, StudentName = student_name,
                AvatarUrl = avatar_url, Score = score, Finished = finished, TurnOrder = turn_order,
                LastSeen = ParseDate(last_seen), UpdatedAt = ParseDate(updated_at)
            };
        }

        [Serializable]
        class ParticipantWriteDto
        {
            public string session_id;
            public string student_id;
            public string student_name;
            public string avatar_url;
            public int score;
            public bool finished;
            public int turn_order;
            public string last_seen;

            public static ParticipantWriteDto From(GameParticipant p) => new ParticipantWriteDto
            {
                session_id = p.SessionId,
                student_id = p.StudentId,
                student_name = p.StudentName,
                avatar_url = string.IsNullOrEmpty(p.AvatarUrl) ? null : p.AvatarUrl,
                score = p.Score,
                finished = p.Finished,
                turn_order = p.TurnOrder,
                last_seen = DateTime.UtcNow.ToString("o")
            };
        }

        [Serializable]
        class EventDto
        {
            public string id;
            public long seq;
            public string session_id;
            public string actor_id;
            public string actor_name;
            public string type;
            public int token_index;
            public int points;
            public string payload;
            public string created_at;

            public GameEvent ToModel() => new GameEvent
            {
                Id = id, Seq = seq, SessionId = session_id, ActorId = actor_id, ActorName = actor_name,
                Type = type, TokenIndex = token_index, Points = points, Payload = payload,
                CreatedAt = ParseDate(created_at)
            };
        }

        [Serializable]
        class EventWriteDto
        {
            public string session_id;
            public string actor_id;
            public string actor_name;
            public string type;
            public int token_index;
            public int points;
            public string payload;

            public static EventWriteDto From(GameEvent e) => new EventWriteDto
            {
                session_id = e.SessionId,
                actor_id = e.ActorId,
                actor_name = e.ActorName,
                type = e.Type,
                token_index = e.TokenIndex,
                points = e.Points,
                payload = string.IsNullOrEmpty(e.Payload) ? null : e.Payload
            };
        }

        [Serializable] class StatusPatch { public string status; }

        static DateTime ParseDate(string value)
            => DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var d)
                ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : DateTime.UtcNow;

        [Serializable] class Wrapper<T> { public T[] items; }

        static T[] ParseArray<T>(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "[]") return Array.Empty<T>();
            try
            {
                var wrapped = JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}");
                return wrapped?.items ?? Array.Empty<T>();
            }
            catch { return Array.Empty<T>(); }
        }
    }
}
