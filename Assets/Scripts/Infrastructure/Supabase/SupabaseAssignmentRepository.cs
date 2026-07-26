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
    /// PostgREST-backed assignment repository over {SupabaseUrl}/rest/v1.
    /// Assignments (assigned to a class) are fully implemented; attempts /
    /// submissions / files remain phase stubs. Calls carry the current user's
    /// access token so Row-Level Security applies (a tutor only writes
    /// assignments for classes in their own academies).
    /// </summary>
    public sealed class SupabaseAssignmentRepository : SupabaseRepositoryBase, IAssignmentRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseAssignmentRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<Assignment>> GetAsync(string assignmentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<Assignment>(configured.Error);

            var url = $"{RestUrl}/assignments?id=eq.{Uri.EscapeDataString(assignmentId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<Assignment>(response.Error);

            var rows = ParseArray<AssignmentDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<Assignment>("Assignment not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<IReadOnlyList<Assignment>>> ListByClassAsync(string classId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<Assignment>>(configured.Error);

            var url = $"{RestUrl}/assignments?class_id=eq.{Uri.EscapeDataString(classId)}&select=*&order=due_date.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<Assignment>>(response.Error);

            var rows = ParseArray<AssignmentDto>(response.Value);
            var list = new List<Assignment>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<Assignment>>(list);
        }

        public async Task<Result> CreateAsync(Assignment assignment, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(AssignmentWriteDto.From(assignment));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/assignments",
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UpdateAsync(Assignment assignment, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(AssignmentWriteDto.From(assignment));
            var url = $"{RestUrl}/assignments?id=eq.{Uri.EscapeDataString(assignment.Id)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteAsync(string assignmentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/assignments?id=eq.{Uri.EscapeDataString(assignmentId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public Task<Result<IReadOnlyList<AssignmentFile>>> ListFilesAsync(string assignmentId, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<AssignmentFile>>("List assignment files");

        public async Task<Result> SetContentAsync(string assignmentId, string contentJson, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = "{\"content_json\":" + JsonString(contentJson) + "}";
            var url = $"{RestUrl}/assignments?id=eq.{Uri.EscapeDataString(assignmentId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Attempts ──────────────────────────────────────────────────────────

        public async Task<Result<AssignmentAttempt>> GetAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AssignmentAttempt>(configured.Error);

            var url = $"{RestUrl}/assignment_attempts?assignment_id=eq.{Uri.EscapeDataString(assignmentId)}&student_id=eq.{Uri.EscapeDataString(studentId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AssignmentAttempt>(response.Error);

            var rows = ParseArray<AttemptDto>(response.Value);
            // No attempt yet is a valid answer, not an error.
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public async Task<Result> UpsertAttemptAsync(AssignmentAttempt attempt, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = "{"
                + "\"assignment_id\":" + JsonString(attempt.AssignmentId) + ","
                + "\"student_id\":" + JsonString(attempt.StudentId) + ","
                + "\"status\":" + JsonString(attempt.Status.ToString().ToLowerInvariant()) + ","
                + "\"started_at\":" + IsoOrNull(attempt.StartedAt) + ","
                + "\"completed_at\":" + IsoOrNull(attempt.CompletedAt) + ","
                + "\"updated_at\":" + IsoOrNull(DateTime.UtcNow)
                + "}";
            var url = $"{RestUrl}/assignment_attempts?on_conflict=assignment_id,student_id";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, url,
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<IReadOnlyList<AssignmentAttempt>>> ListAttemptsForStudentAsync(string studentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AssignmentAttempt>>(configured.Error);

            var url = $"{RestUrl}/assignment_attempts?student_id=eq.{Uri.EscapeDataString(studentId)}&select=*";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AssignmentAttempt>>(response.Error);

            var rows = ParseArray<AttemptDto>(response.Value);
            var list = new List<AssignmentAttempt>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AssignmentAttempt>>(list);
        }

        // ── Submissions ───────────────────────────────────────────────────────

        public async Task<Result> CreateSubmissionAsync(AssignmentSubmission submission, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(SubmissionWriteDto.From(submission));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/assignment_submissions",
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<AssignmentSubmission>> GetSubmissionAsync(string attemptId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AssignmentSubmission>(configured.Error);

            var url = $"{RestUrl}/assignment_submissions?attempt_id=eq.{Uri.EscapeDataString(attemptId)}&select=*&order=submitted_at.desc&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AssignmentSubmission>(response.Error);

            var rows = ParseArray<SubmissionDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public async Task<Result> GradeSubmissionAsync(string submissionId, int grade, string feedback, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new GradePatch { grade = grade, feedback = feedback });
            var url = $"{RestUrl}/assignment_submissions?id=eq.{Uri.EscapeDataString(submissionId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Schedules ─────────────────────────────────────────────────────────

        public async Task<Result> UpsertScheduleAsync(AssignmentSchedule s, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = "{"
                + "\"assignment_id\":" + JsonString(s.AssignmentId) + ","
                + "\"student_id\":" + JsonString(s.StudentId) + ","
                + "\"student_name\":" + JsonString(s.StudentName) + ","
                + "\"class_id\":" + JsonString(s.ClassId) + ","
                + "\"class_name\":" + JsonString(s.ClassName) + ","
                + "\"assignment_title\":" + JsonString(s.AssignmentTitle) + ","
                + "\"scheduled_for\":" + IsoOrNull(s.ScheduledFor) + ","
                + "\"hidden\":" + (s.Hidden ? "true" : "false") + ","
                + "\"note\":" + JsonString(s.Note) + ","
                + "\"updated_at\":" + IsoOrNull(DateTime.UtcNow)
                + "}";
            var url = $"{RestUrl}/assignment_schedules?on_conflict=assignment_id,student_id";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, url,
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteScheduleAsync(string scheduleId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/assignment_schedules?id=eq.{Uri.EscapeDataString(scheduleId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public Task<Result<IReadOnlyList<AssignmentSchedule>>> ListMySchedulesAsync(string studentId, CancellationToken ct = default)
            => QuerySchedules($"{RestUrl}/assignment_schedules?student_id=eq.{Uri.EscapeDataString(studentId)}&select=*&order=scheduled_for.asc", ct);

        public Task<Result<IReadOnlyList<AssignmentSchedule>>> ListSchedulesForAssignmentAsync(string assignmentId, CancellationToken ct = default)
            => QuerySchedules($"{RestUrl}/assignment_schedules?assignment_id=eq.{Uri.EscapeDataString(assignmentId)}&select=*&order=scheduled_for.asc", ct);

        async Task<Result<IReadOnlyList<AssignmentSchedule>>> QuerySchedules(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AssignmentSchedule>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AssignmentSchedule>>(response.Error);

            var rows = ParseArray<ScheduleDto>(response.Value);
            var list = new List<AssignmentSchedule>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AssignmentSchedule>>(list);
        }

        // ── PostgREST DTOs ────────────────────────────────────────────────────

        [Serializable]
        class AssignmentDto
        {
            public string id;
            public string class_id;
            public string title;
            public string instructions;
            public string content_json;
            public string due_date;
            public int max_score;
            public int version;
            public string created_at;
            public string updated_at;

            public Assignment ToModel() => new Assignment
            {
                Id           = id,
                ClassId      = class_id,
                Title        = title,
                Instructions = instructions,
                ContentJson  = content_json,
                DueDate      = ParseDate(due_date),
                MaxScore     = max_score,
                Version      = version,
                CreatedAt    = ParseDate(created_at),
                UpdatedAt    = ParseDate(updated_at)
            };
        }

        [Serializable]
        class AttemptDto
        {
            public string id;
            public string assignment_id;
            public string student_id;
            public string status;
            public string started_at;
            public string completed_at;

            public AssignmentAttempt ToModel() => new AssignmentAttempt
            {
                Id           = id,
                AssignmentId = assignment_id,
                StudentId    = student_id,
                Status       = ParseEnum(status, AttemptStatus.InProgress),
                StartedAt    = string.IsNullOrEmpty(started_at) ? (DateTime?)null : ParseDate(started_at),
                CompletedAt  = string.IsNullOrEmpty(completed_at) ? (DateTime?)null : ParseDate(completed_at)
            };
        }

        [Serializable]
        class SubmissionDto
        {
            public string id;
            public string attempt_id;
            public string assignment_id;
            public string student_id;
            public string submitted_at;
            public string file_path;
            public int grade;
            public string feedback;

            public AssignmentSubmission ToModel() => new AssignmentSubmission
            {
                Id           = id,
                AttemptId    = attempt_id,
                AssignmentId = assignment_id,
                StudentId    = student_id,
                SubmittedAt  = ParseDate(submitted_at),
                FilePath     = file_path,
                Grade        = grade,
                Feedback     = feedback
            };
        }

        [Serializable]
        class SubmissionWriteDto
        {
            public string attempt_id;
            public string assignment_id;
            public string student_id;
            public string file_path;

            public static SubmissionWriteDto From(AssignmentSubmission s) => new SubmissionWriteDto
            {
                attempt_id    = s.AttemptId,
                assignment_id = s.AssignmentId,
                student_id    = s.StudentId,
                file_path     = s.FilePath
            };
        }

        [Serializable] class GradePatch { public int grade; public string feedback; }

        [Serializable]
        class ScheduleDto
        {
            public string id;
            public string assignment_id;
            public string student_id;
            public string student_name;
            public string class_id;
            public string class_name;
            public string assignment_title;
            public string scheduled_for;
            public bool hidden;
            public string note;
            public string created_at;
            public string updated_at;

            public AssignmentSchedule ToModel() => new AssignmentSchedule
            {
                Id              = id,
                AssignmentId    = assignment_id,
                StudentId       = student_id,
                StudentName     = student_name,
                ClassId         = class_id,
                ClassName       = class_name,
                AssignmentTitle = assignment_title,
                ScheduledFor    = ParseDate(scheduled_for),
                Hidden          = hidden,
                Note            = note,
                CreatedAt       = ParseDate(created_at),
                UpdatedAt       = ParseDate(updated_at)
            };
        }

        [Serializable]
        class AssignmentWriteDto
        {
            public string class_id;
            public string title;
            public string instructions;
            public string due_date;
            public int max_score;
            public int version;

            public static AssignmentWriteDto From(Assignment a) => new AssignmentWriteDto
            {
                class_id     = a.ClassId,
                title        = a.Title,
                instructions = a.Instructions,
                due_date     = a.DueDate.ToUniversalTime().ToString("o"),
                max_score    = a.MaxScore,
                version      = a.Version <= 0 ? 1 : a.Version
            };
        }

        static DateTime ParseDate(string value)
            => DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var d)
                ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : DateTime.UtcNow;

        static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct
            => Enum.TryParse<TEnum>(value, true, out var parsed) ? parsed : fallback;

        // A JSON string literal, or the literal null when empty (so a uuid/text
        // column gets real null instead of "" — JsonUtility can't emit null).
        static string JsonString(string s)
        {
            if (string.IsNullOrEmpty(s)) return "null";
            var sb = new System.Text.StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:   sb.Append(ch);     break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        // An ISO-8601 timestamp literal, or null when there's no value.
        static string IsoOrNull(DateTime? value)
            => value.HasValue ? "\"" + value.Value.ToUniversalTime().ToString("o") + "\"" : "null";

        // JsonUtility cannot parse a top-level JSON array, so wrap it first.
        [Serializable] class Wrapper<T> { public T[] items; }

        static T[] ParseArray<T>(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "[]") return Array.Empty<T>();
            try
            {
                var wrapped = JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}");
                return wrapped?.items ?? Array.Empty<T>();
            }
            catch
            {
                return Array.Empty<T>();
            }
        }
    }
}
