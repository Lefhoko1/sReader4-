using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Repositories;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// A local mirror row for one assignment schedule. <see cref="PendingOp"/>
    /// records an unsynced local change. Offline-created rows get a deterministic
    /// "local_{assignment}_{student}" id so a re-save offline updates the same row;
    /// the real uuid replaces it on the next online refresh.
    /// </summary>
    [Table("assignment_schedules")]
    public sealed class AssignmentScheduleRow
    {
        [PrimaryKey] public string Id { get; set; }
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string AssignmentTitle { get; set; }
        public long ScheduledForTicks { get; set; }
        public bool Hidden { get; set; }
        public string Note { get; set; }
        public string PendingOp { get; set; }

        public const string Synced = "";
        public const string Create = "create";
        public const string Update = "update";
        public const string Delete = "delete";

        public static string LocalKey(string assignmentId, string studentId) => "local_" + assignmentId + "_" + studentId;

        public static AssignmentScheduleRow From(AssignmentSchedule s, string pendingOp) => new AssignmentScheduleRow
        {
            Id                = string.IsNullOrEmpty(s.Id) ? LocalKey(s.AssignmentId, s.StudentId) : s.Id,
            AssignmentId      = s.AssignmentId,
            StudentId         = s.StudentId,
            StudentName       = s.StudentName,
            ClassId           = s.ClassId,
            ClassName         = s.ClassName,
            AssignmentTitle   = s.AssignmentTitle,
            ScheduledForTicks = s.ScheduledFor.Ticks,
            Hidden            = s.Hidden,
            Note              = s.Note,
            PendingOp         = pendingOp ?? Synced
        };

        public AssignmentSchedule ToModel() => new AssignmentSchedule
        {
            Id              = Id != null && Id.StartsWith("local_") ? null : Id,
            AssignmentId    = AssignmentId,
            StudentId       = StudentId,
            StudentName     = StudentName,
            ClassId         = ClassId,
            ClassName       = ClassName,
            AssignmentTitle = AssignmentTitle,
            ScheduledFor    = new DateTime(ScheduledForTicks, DateTimeKind.Utc),
            Hidden          = Hidden,
            Note            = Note
        };
    }

    /// <summary>
    /// A local mirror row for one student's attempt on an assignment. Keyed by
    /// "{assignmentId}|{studentId}" — the SAME pair the server upserts on — so
    /// there is always exactly one row per attempt and a re-save offline updates
    /// it in place. <see cref="ServerId"/> is filled once the real uuid is known
    /// (from an online get/list), and <see cref="PendingOp"/> marks an unsynced
    /// local change to push on the next online read.
    /// </summary>
    [Table("assignment_attempts")]
    public sealed class AssignmentAttemptRow
    {
        [PrimaryKey] public string Key { get; set; }   // "{assignmentId}|{studentId}"
        public string ServerId { get; set; }            // real uuid once known
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }              // AttemptStatus name
        public long StartedAtTicks { get; set; }        // 0 = null
        public long CompletedAtTicks { get; set; }      // 0 = null
        public string PendingOp { get; set; }

        public const string Synced = "";
        public const string Create = "create";
        public const string Update = "update";

        public static string KeyOf(string assignmentId, string studentId)
            => (assignmentId ?? "") + "|" + (studentId ?? "");

        public static AssignmentAttemptRow From(AssignmentAttempt a, string pendingOp) => new AssignmentAttemptRow
        {
            Key              = KeyOf(a.AssignmentId, a.StudentId),
            ServerId         = a.Id,
            AssignmentId     = a.AssignmentId,
            StudentId        = a.StudentId,
            Status           = a.Status.ToString(),
            StartedAtTicks   = a.StartedAt.HasValue ? a.StartedAt.Value.Ticks : 0L,
            CompletedAtTicks = a.CompletedAt.HasValue ? a.CompletedAt.Value.Ticks : 0L,
            PendingOp        = pendingOp ?? Synced
        };

        public AssignmentAttempt ToModel() => new AssignmentAttempt
        {
            Id           = ServerId,
            AssignmentId = AssignmentId,
            StudentId    = StudentId,
            Status       = Enum.TryParse<AttemptStatus>(Status, true, out var s) ? s : AttemptStatus.InProgress,
            StartedAt    = StartedAtTicks   > 0 ? new DateTime(StartedAtTicks,   DateTimeKind.Utc) : (DateTime?)null,
            CompletedAt  = CompletedAtTicks > 0 ? new DateTime(CompletedAtTicks, DateTimeKind.Utc) : (DateTime?)null
        };
    }

    /// <summary>
    /// An outbox row for a submission made offline (e.g. a finished game's score).
    /// It is created locally and pushed — then removed — on the next online read;
    /// the real <c>attempt_id</c> is resolved from the server at flush time, since
    /// an offline attempt has no server id yet.
    /// </summary>
    [Table("assignment_submissions_outbox")]
    public sealed class AssignmentSubmissionRow
    {
        [PrimaryKey] public string Id { get; set; }     // local guid
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }
        public long SubmittedAtTicks { get; set; }
        public string FilePath { get; set; }

        public static AssignmentSubmissionRow From(AssignmentSubmission s) => new AssignmentSubmissionRow
        {
            Id               = string.IsNullOrEmpty(s.Id) ? Guid.NewGuid().ToString("N") : s.Id,
            AssignmentId     = s.AssignmentId,
            StudentId        = s.StudentId,
            SubmittedAtTicks = s.SubmittedAt == default ? DateTime.UtcNow.Ticks : s.SubmittedAt.Ticks,
            FilePath         = s.FilePath
        };

        public AssignmentSubmission ToModel() => new AssignmentSubmission
        {
            AssignmentId = AssignmentId,
            StudentId    = StudentId,
            SubmittedAt  = SubmittedAtTicks > 0 ? new DateTime(SubmittedAtTicks, DateTimeKind.Utc) : DateTime.UtcNow,
            FilePath     = FilePath
        };
    }

    /// <summary>
    /// Offline-first decorator over the Supabase assignment repository so a student
    /// can play their assignments alone without a connection. SCHEDULES and ATTEMPTS
    /// are mirrored in SQLite: reads serve the live server when online (refreshing
    /// the mirror) and the local mirror when offline; writes go straight to Supabase
    /// when online, else are queued and pushed on the next online read. Submissions
    /// made offline (a finished game's score) go to a local outbox and are flushed
    /// the same way. The remaining reads (assignments, content, files) pass through —
    /// they are already cached at the HTTP layer and primed by the cache warmer.
    /// </summary>
    public sealed class OfflineFirstAssignmentRepository : IAssignmentRepository
    {
        readonly IAssignmentRepository inner;
        readonly SqliteDatabase db;
        readonly IConnectivity connectivity;
        readonly IAppLogger logger;

        bool flushing;

        public OfflineFirstAssignmentRepository(IAssignmentRepository inner, SqliteDatabase db,
            IConnectivity connectivity, IAppLogger logger)
        {
            this.inner        = Guard.NotNull(inner, nameof(inner));
            this.db           = Guard.NotNull(db, nameof(db));
            this.connectivity = Guard.NotNull(connectivity, nameof(connectivity));
            this.logger       = Guard.NotNull(logger, nameof(logger));
        }

        // ── Schedules (offline-first) ────────────────────────────────────────

        public async Task<Result> UpsertScheduleAsync(AssignmentSchedule schedule, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var saved = await inner.UpsertScheduleAsync(schedule, ct);
                if (saved.IsSuccess) return saved;  // the real row arrives on the next list refresh
                logger.Warning($"[Offline] Save schedule failed online, queuing locally: {saved.Error}");
            }

            var existing = FindPair(schedule.AssignmentId, schedule.StudentId);
            var op = existing != null && existing.PendingOp == AssignmentScheduleRow.Create
                ? AssignmentScheduleRow.Create
                : (existing == null ? AssignmentScheduleRow.Create : AssignmentScheduleRow.Update);
            var row = AssignmentScheduleRow.From(schedule, op);
            if (existing != null) row.Id = existing.Id;
            Upsert(row);
            return Result.Ok();
        }

        public async Task<Result> DeleteScheduleAsync(string scheduleId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(scheduleId)) return Result.Fail("Schedule id is required.");

            if (connectivity.IsOnline)
            {
                var deleted = await inner.DeleteScheduleAsync(scheduleId, ct);
                if (deleted.IsSuccess) { Remove(scheduleId); return deleted; }
                logger.Warning($"[Offline] Delete schedule failed online, queuing locally: {deleted.Error}");
            }

            var row = Find(scheduleId);
            if (row == null) return Result.Ok();
            if (row.PendingOp == AssignmentScheduleRow.Create) Remove(scheduleId); // never reached the server
            else { row.PendingOp = AssignmentScheduleRow.Delete; Upsert(row); }
            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<AssignmentSchedule>>> ListMySchedulesAsync(string studentId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListMySchedulesAsync(studentId, ct);
                if (remote.IsSuccess) { RefreshMirror(remote.Value, studentId); return remote; }
                logger.Warning($"[Offline] List my schedules failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalFor(studentId));
        }

        public Task<Result<IReadOnlyList<AssignmentSchedule>>> ListSchedulesForAssignmentAsync(string assignmentId, CancellationToken ct = default)
            => inner.ListSchedulesForAssignmentAsync(assignmentId, ct);   // cached at the HTTP layer

        /// <summary>Push every queued change (schedules, attempts, submissions) to
        /// Supabase. Best-effort, re-entrant-safe; attempts are flushed before
        /// submissions so an offline submission can resolve its server attempt id.</summary>
        public async Task FlushPendingAsync(CancellationToken ct = default)
        {
            if (flushing || !connectivity.IsOnline) return;
            flushing = true;
            try
            {
                await FlushSchedulesAsync(ct);
                await FlushAttemptsAsync(ct);
                await FlushSubmissionsAsync(ct);
            }
            finally
            {
                flushing = false;
            }
        }

        async Task FlushSchedulesAsync(CancellationToken ct)
        {
            List<AssignmentScheduleRow> pending;
            lock (db.Gate)
            {
                pending = db.Connection.Table<AssignmentScheduleRow>()
                    .Where(r => r.PendingOp != AssignmentScheduleRow.Synced)
                    .ToList();
            }

            foreach (var row in pending)
            {
                if (ct.IsCancellationRequested) break;
                if (row.PendingOp == AssignmentScheduleRow.Delete)
                {
                    // A row that never synced has only a local id — just forget it.
                    if (row.Id != null && row.Id.StartsWith("local_")) { Remove(row.Id); continue; }
                    var deleted = await inner.DeleteScheduleAsync(row.Id, ct);
                    if (deleted.IsSuccess) Remove(row.Id);
                    else logger.Warning($"[Offline] Flush delete schedule deferred: {deleted.Error}");
                }
                else
                {
                    var saved = await inner.UpsertScheduleAsync(row.ToModel(), ct);
                    if (saved.IsSuccess) Remove(row.Id);  // real row arrives on the next refresh
                    else logger.Warning($"[Offline] Flush save schedule deferred: {saved.Error}");
                }
            }
        }

        async Task FlushAttemptsAsync(CancellationToken ct)
        {
            List<AssignmentAttemptRow> pending;
            lock (db.Gate)
            {
                pending = db.Connection.Table<AssignmentAttemptRow>()
                    .Where(r => r.PendingOp != AssignmentAttemptRow.Synced)
                    .ToList();
            }

            foreach (var row in pending)
            {
                if (ct.IsCancellationRequested) break;
                var saved = await inner.UpsertAttemptAsync(row.ToModel(), ct);
                if (saved.IsSuccess) MarkAttemptSynced(row.Key);  // real id arrives on the next refresh
                else logger.Warning($"[Offline] Flush save attempt deferred: {saved.Error}");
            }
        }

        async Task FlushSubmissionsAsync(CancellationToken ct)
        {
            List<AssignmentSubmissionRow> pending;
            lock (db.Gate) pending = db.Connection.Table<AssignmentSubmissionRow>().ToList();

            foreach (var row in pending)
            {
                if (ct.IsCancellationRequested) break;

                // The attempt has just been flushed; fetch its real id for the FK.
                var attempt = await inner.GetAttemptAsync(row.AssignmentId, row.StudentId, ct);
                if (attempt.IsFailure || attempt.Value == null || string.IsNullOrEmpty(attempt.Value.Id))
                {
                    logger.Warning("[Offline] Flush submission deferred: attempt not yet on the server.");
                    continue;
                }

                var submission = row.ToModel();
                submission.AttemptId = attempt.Value.Id;
                var created = await inner.CreateSubmissionAsync(submission, ct);
                if (created.IsSuccess) RemoveSubmission(row.Id);
                else logger.Warning($"[Offline] Flush submission deferred: {created.Error}");
            }
        }

        // ── SQLite mirror helpers ────────────────────────────────────────────

        AssignmentScheduleRow Find(string id)
        {
            lock (db.Gate) return db.Connection.Find<AssignmentScheduleRow>(id);
        }

        AssignmentScheduleRow FindPair(string assignmentId, string studentId)
        {
            lock (db.Gate)
                return db.Connection.Table<AssignmentScheduleRow>()
                    .FirstOrDefault(r => r.AssignmentId == assignmentId && r.StudentId == studentId);
        }

        void Upsert(AssignmentScheduleRow row)
        {
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        void Remove(string id)
        {
            lock (db.Gate) db.Connection.Delete<AssignmentScheduleRow>(id);
        }

        IReadOnlyList<AssignmentSchedule> LocalFor(string studentId)
        {
            List<AssignmentScheduleRow> rows;
            lock (db.Gate)
                rows = db.Connection.Table<AssignmentScheduleRow>()
                    .Where(r => r.StudentId == studentId && r.PendingOp != AssignmentScheduleRow.Delete)
                    .ToList();
            return rows.OrderBy(r => r.ScheduledForTicks).Select(r => r.ToModel()).ToList();
        }

        void RefreshMirror(IReadOnlyList<AssignmentSchedule> remote, string studentId)
        {
            lock (db.Gate)
            {
                foreach (var s in remote)
                {
                    if (string.IsNullOrEmpty(s.Id)) continue;
                    var existing = db.Connection.Find<AssignmentScheduleRow>(s.Id);
                    if (existing != null && existing.PendingOp != AssignmentScheduleRow.Synced) continue;
                    db.Connection.InsertOrReplace(AssignmentScheduleRow.From(s, AssignmentScheduleRow.Synced));
                }
            }
        }

        // ── Attempts (offline-first) ─────────────────────────────────────────

        public async Task<Result<AssignmentAttempt>> GetAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var remote = await inner.GetAttemptAsync(assignmentId, studentId, ct);
                if (remote.IsSuccess)
                {
                    if (remote.Value != null) RefreshAttempt(remote.Value);
                    return remote;
                }
                logger.Warning($"[Offline] Get attempt failed online, serving local mirror: {remote.Error}");
            }
            var row = FindAttempt(assignmentId, studentId);
            return Result.Ok(row?.ToModel());   // no attempt yet is a valid answer
        }

        public async Task<Result> UpsertAttemptAsync(AssignmentAttempt attempt, CancellationToken ct = default)
        {
            if (attempt == null) return Result.Fail("An attempt is required.");

            if (connectivity.IsOnline)
            {
                var saved = await inner.UpsertAttemptAsync(attempt, ct);
                if (saved.IsSuccess)
                {
                    UpsertAttempt(AssignmentAttemptRow.From(attempt, AssignmentAttemptRow.Synced));
                    return saved;
                }
                logger.Warning($"[Offline] Save attempt failed online, queuing locally: {saved.Error}");
            }

            var existing = FindAttempt(attempt.AssignmentId, attempt.StudentId);
            var op = existing != null && existing.PendingOp == AssignmentAttemptRow.Create
                ? AssignmentAttemptRow.Create
                : (existing == null ? AssignmentAttemptRow.Create : AssignmentAttemptRow.Update);
            var row = AssignmentAttemptRow.From(attempt, op);
            if (existing != null && string.IsNullOrEmpty(row.ServerId)) row.ServerId = existing.ServerId;
            UpsertAttempt(row);
            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<AssignmentAttempt>>> ListAttemptsForStudentAsync(string studentId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListAttemptsForStudentAsync(studentId, ct);
                if (remote.IsSuccess) { RefreshAttempts(remote.Value); return remote; }
                logger.Warning($"[Offline] List attempts failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalAttemptsFor(studentId));
        }

        // ── Submissions (offline outbox) ─────────────────────────────────────

        public async Task<Result> CreateSubmissionAsync(AssignmentSubmission submission, CancellationToken ct = default)
        {
            if (submission == null) return Result.Fail("A submission is required.");

            if (connectivity.IsOnline)
            {
                var created = await inner.CreateSubmissionAsync(submission, ct);
                if (created.IsSuccess) return created;
                logger.Warning($"[Offline] Create submission failed online, queuing locally: {created.Error}");
            }

            lock (db.Gate) db.Connection.InsertOrReplace(AssignmentSubmissionRow.From(submission));
            return Result.Ok();
        }

        // ── Attempt mirror helpers ───────────────────────────────────────────

        AssignmentAttemptRow FindAttempt(string assignmentId, string studentId)
        {
            lock (db.Gate) return db.Connection.Find<AssignmentAttemptRow>(AssignmentAttemptRow.KeyOf(assignmentId, studentId));
        }

        void UpsertAttempt(AssignmentAttemptRow row)
        {
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        void MarkAttemptSynced(string key)
        {
            lock (db.Gate)
            {
                var row = db.Connection.Find<AssignmentAttemptRow>(key);
                if (row == null || row.PendingOp == AssignmentAttemptRow.Synced) return;
                row.PendingOp = AssignmentAttemptRow.Synced;
                db.Connection.InsertOrReplace(row);
            }
        }

        IReadOnlyList<AssignmentAttempt> LocalAttemptsFor(string studentId)
        {
            List<AssignmentAttemptRow> rows;
            lock (db.Gate)
                rows = db.Connection.Table<AssignmentAttemptRow>()
                    .Where(r => r.StudentId == studentId)
                    .ToList();
            return rows.Select(r => r.ToModel()).ToList();
        }

        // Mirror one server attempt, learning its real id but never clobbering an
        // unsynced local change for the same pair.
        void RefreshAttempt(AssignmentAttempt remote)
        {
            if (remote == null || string.IsNullOrEmpty(remote.AssignmentId)) return;
            lock (db.Gate)
            {
                var key = AssignmentAttemptRow.KeyOf(remote.AssignmentId, remote.StudentId);
                var existing = db.Connection.Find<AssignmentAttemptRow>(key);
                if (existing != null && existing.PendingOp != AssignmentAttemptRow.Synced)
                {
                    if (string.IsNullOrEmpty(existing.ServerId) && !string.IsNullOrEmpty(remote.Id))
                    {
                        existing.ServerId = remote.Id;   // learn the id; keep the pending change
                        db.Connection.InsertOrReplace(existing);
                    }
                    return;
                }
                db.Connection.InsertOrReplace(AssignmentAttemptRow.From(remote, AssignmentAttemptRow.Synced));
            }
        }

        void RefreshAttempts(IReadOnlyList<AssignmentAttempt> remote)
        {
            if (remote == null) return;
            foreach (var a in remote) RefreshAttempt(a);
        }

        void RemoveSubmission(string id)
        {
            lock (db.Gate) db.Connection.Delete<AssignmentSubmissionRow>(id);
        }

        // ── Pass-through (assignments, content, files, grading) ──────────────

        public Task<Result<Assignment>> GetAsync(string assignmentId, CancellationToken ct = default)
            => inner.GetAsync(assignmentId, ct);

        public Task<Result<IReadOnlyList<Assignment>>> ListByClassAsync(string classId, CancellationToken ct = default)
            => inner.ListByClassAsync(classId, ct);

        public Task<Result> CreateAsync(Assignment assignment, CancellationToken ct = default)
            => inner.CreateAsync(assignment, ct);

        public Task<Result> UpdateAsync(Assignment assignment, CancellationToken ct = default)
            => inner.UpdateAsync(assignment, ct);

        public Task<Result> DeleteAsync(string assignmentId, CancellationToken ct = default)
            => inner.DeleteAsync(assignmentId, ct);

        public Task<Result> SetContentAsync(string assignmentId, string contentJson, CancellationToken ct = default)
            => inner.SetContentAsync(assignmentId, contentJson, ct);

        public Task<Result<IReadOnlyList<AssignmentFile>>> ListFilesAsync(string assignmentId, CancellationToken ct = default)
            => inner.ListFilesAsync(assignmentId, ct);

        public Task<Result<AssignmentSubmission>> GetSubmissionAsync(string attemptId, CancellationToken ct = default)
            => inner.GetSubmissionAsync(attemptId, ct);

        public Task<Result> GradeSubmissionAsync(string submissionId, int grade, string feedback, CancellationToken ct = default)
            => inner.GradeSubmissionAsync(submissionId, grade, feedback, ct);
    }
}
