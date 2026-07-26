using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Education.Models;
using SReader.Domains.Education.Repositories;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// A local mirror row for one academy. <see cref="PendingOp"/> records an
    /// unsynced local change: "" (synced), "create", "update" or "delete".
    /// Offline-created rows get a temporary "local_…" id until the server
    /// assigns the real uuid on the next flush.
    /// </summary>
    [Table("academies")]
    public sealed class AcademyRow
    {
        [PrimaryKey] public string Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public long CreatedAtTicks { get; set; }
        public string PendingOp { get; set; }

        public const string Synced = "";
        public const string Create = "create";
        public const string Update = "update";
        public const string Delete = "delete";

        public static AcademyRow From(Academy a, string pendingOp) => new AcademyRow
        {
            Id             = a.Id,
            OwnerId        = a.OwnerId,
            Name           = a.Name,
            Description    = a.Description,
            City           = a.City,
            Country        = a.Country,
            CreatedAtTicks = a.CreatedAt.Ticks,
            PendingOp      = pendingOp ?? Synced
        };

        public Academy ToModel() => new Academy
        {
            Id          = Id,
            OwnerId     = OwnerId,
            Name        = Name,
            Description = Description,
            City        = City,
            Country     = Country,
            CreatedAt   = new DateTime(CreatedAtTicks, DateTimeKind.Utc)
        };
    }

    /// <summary>
    /// Offline-first decorator over the Supabase education repository, for
    /// academies only. Reads serve the live server when online (refreshing the
    /// local mirror) and the SQLite mirror when offline or when the request
    /// fails. Writes go straight to Supabase when online; offline they are
    /// recorded in the mirror with a PendingOp and pushed on the next online
    /// read via <see cref="FlushPendingAsync"/>. Every other repository call
    /// (grades, courses, join requests, enrollments…) passes through unchanged.
    /// </summary>
    public sealed class OfflineFirstEducationRepository : IEducationRepository
    {
        readonly IEducationRepository inner;
        readonly SqliteDatabase db;
        readonly IConnectivity connectivity;
        readonly IAppLogger logger;

        bool flushing;

        public OfflineFirstEducationRepository(IEducationRepository inner, SqliteDatabase db,
            IConnectivity connectivity, IAppLogger logger)
        {
            this.inner        = Guard.NotNull(inner, nameof(inner));
            this.db           = Guard.NotNull(db, nameof(db));
            this.connectivity = Guard.NotNull(connectivity, nameof(connectivity));
            this.logger       = Guard.NotNull(logger, nameof(logger));
        }

        // ── Academies (offline-first) ───────────────────────────────────────

        public async Task<Result<IReadOnlyList<Academy>>> ListAcademiesAsync(CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListAcademiesAsync(ct);
                if (remote.IsSuccess)
                {
                    RefreshMirror(remote.Value);
                    return remote;
                }
                logger.Warning($"[Offline] List academies failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalAcademies(ownerId: null));
        }

        public async Task<Result<IReadOnlyList<Academy>>> ListAcademiesByOwnerAsync(string ownerId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListAcademiesByOwnerAsync(ownerId, ct);
                if (remote.IsSuccess)
                {
                    RefreshMirror(remote.Value);
                    return remote;
                }
                logger.Warning($"[Offline] List my academies failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalAcademies(ownerId));
        }

        public async Task<Result<Academy>> GetAcademyAsync(string academyId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var remote = await inner.GetAcademyAsync(academyId, ct);
                if (remote.IsSuccess)
                {
                    Upsert(AcademyRow.From(remote.Value, AcademyRow.Synced));
                    return remote;
                }
                logger.Warning($"[Offline] Get academy failed online, serving local mirror: {remote.Error}");
            }

            var row = Find(academyId);
            return row == null || row.PendingOp == AcademyRow.Delete
                ? Result.Fail<Academy>("Academy not found.")
                : Result.Ok(row.ToModel());
        }

        public async Task<Result<Academy>> CreateAcademyAsync(Academy academy, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var created = await inner.CreateAcademyAsync(academy, ct);
                if (created.IsSuccess)
                {
                    Upsert(AcademyRow.From(created.Value, AcademyRow.Synced));
                    return created;
                }
                logger.Warning($"[Offline] Create academy failed online, saving locally: {created.Error}");
            }

            // Offline (or the online attempt failed): keep it locally and queue it.
            if (string.IsNullOrEmpty(academy.Id))
                academy.Id = "local_" + Guid.NewGuid().ToString("N");
            Upsert(AcademyRow.From(academy, AcademyRow.Create));
            return Result.Ok(academy);
        }

        public async Task<Result> UpdateAcademyAsync(Academy academy, CancellationToken ct = default)
        {
            if (academy == null || string.IsNullOrEmpty(academy.Id))
                return Result.Fail("An academy with an id is required.");

            if (connectivity.IsOnline)
            {
                var updated = await inner.UpdateAcademyAsync(academy, ct);
                if (updated.IsSuccess)
                {
                    Upsert(AcademyRow.From(academy, AcademyRow.Synced));
                    return updated;
                }
                logger.Warning($"[Offline] Update academy failed online, saving locally: {updated.Error}");
            }

            // A row still pending creation stays "create"; otherwise it's an update.
            var existing = Find(academy.Id);
            var op = existing != null && existing.PendingOp == AcademyRow.Create ? AcademyRow.Create : AcademyRow.Update;
            Upsert(AcademyRow.From(academy, op));
            return Result.Ok();
        }

        public async Task<Result> DeleteAcademyAsync(string academyId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(academyId)) return Result.Fail("Academy id is required.");

            if (connectivity.IsOnline)
            {
                var deleted = await inner.DeleteAcademyAsync(academyId, ct);
                if (deleted.IsSuccess)
                {
                    Remove(academyId);
                    return deleted;
                }
                logger.Warning($"[Offline] Delete academy failed online, saving locally: {deleted.Error}");
            }

            var row = Find(academyId);
            if (row == null) return Result.Ok();

            if (row.PendingOp == AcademyRow.Create)
                Remove(academyId);          // never reached the server — just forget it
            else
            {
                row.PendingOp = AcademyRow.Delete;
                Upsert(row);
            }
            return Result.Ok();
        }

        /// <summary>
        /// Push every queued academy change to Supabase. Best-effort: a failed
        /// op is left pending and retried on the next flush. Re-entrant-safe.
        /// </summary>
        public async Task FlushPendingAsync(CancellationToken ct = default)
        {
            if (flushing || !connectivity.IsOnline) return;
            flushing = true;
            try
            {
                List<AcademyRow> pending;
                lock (db.Gate)
                {
                    pending = db.Connection.Table<AcademyRow>()
                        .Where(r => r.PendingOp != AcademyRow.Synced)
                        .ToList();
                }

                foreach (var row in pending)
                {
                    if (ct.IsCancellationRequested) break;

                    switch (row.PendingOp)
                    {
                        case AcademyRow.Create:
                        {
                            var created = await inner.CreateAcademyAsync(row.ToModel(), ct);
                            if (created.IsSuccess)
                            {
                                Remove(row.Id);     // drop the temporary "local_…" id
                                Upsert(AcademyRow.From(created.Value, AcademyRow.Synced));
                            }
                            else logger.Warning($"[Offline] Flush create '{row.Id}' deferred: {created.Error}");
                            break;
                        }
                        case AcademyRow.Update:
                        {
                            var updated = await inner.UpdateAcademyAsync(row.ToModel(), ct);
                            if (updated.IsSuccess) { row.PendingOp = AcademyRow.Synced; Upsert(row); }
                            else logger.Warning($"[Offline] Flush update '{row.Id}' deferred: {updated.Error}");
                            break;
                        }
                        case AcademyRow.Delete:
                        {
                            var deleted = await inner.DeleteAcademyAsync(row.Id, ct);
                            if (deleted.IsSuccess) Remove(row.Id);
                            else logger.Warning($"[Offline] Flush delete '{row.Id}' deferred: {deleted.Error}");
                            break;
                        }
                    }
                }
            }
            finally
            {
                flushing = false;
            }
        }

        // ── SQLite mirror helpers ───────────────────────────────────────────

        AcademyRow Find(string id)
        {
            lock (db.Gate) return db.Connection.Find<AcademyRow>(id);
        }

        void Upsert(AcademyRow row)
        {
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        void Remove(string id)
        {
            lock (db.Gate) db.Connection.Delete<AcademyRow>(id);
        }

        /// <summary>All locally-known academies (optionally one owner), hiding rows pending deletion.</summary>
        IReadOnlyList<Academy> LocalAcademies(string ownerId)
        {
            List<AcademyRow> rows;
            lock (db.Gate)
            {
                var query = db.Connection.Table<AcademyRow>().Where(r => r.PendingOp != AcademyRow.Delete);
                if (!string.IsNullOrEmpty(ownerId)) query = query.Where(r => r.OwnerId == ownerId);
                rows = query.ToList();
            }
            return rows.OrderByDescending(r => r.CreatedAtTicks).Select(r => r.ToModel()).ToList();
        }

        /// <summary>
        /// Replace mirror rows with a fresh server list, but never clobber a row
        /// that still has unsynced local changes (those win until flushed).
        /// </summary>
        void RefreshMirror(IReadOnlyList<Academy> remote)
        {
            lock (db.Gate)
            {
                foreach (var academy in remote)
                {
                    var existing = db.Connection.Find<AcademyRow>(academy.Id);
                    if (existing != null && existing.PendingOp != AcademyRow.Synced) continue;
                    db.Connection.InsertOrReplace(AcademyRow.From(academy, AcademyRow.Synced));
                }
            }
        }

        // ── Pass-through (grades, courses, join requests, enrollments, classes) ─

        public Task<Result> CreateJoinRequestAsync(AcademyJoinRequest request, CancellationToken ct = default)
            => inner.CreateJoinRequestAsync(request, ct);

        public Task<Result<IReadOnlyList<AcademyJoinRequest>>> ListJoinRequestsForOwnerAsync(CancellationToken ct = default)
            => inner.ListJoinRequestsForOwnerAsync(ct);

        public Task<Result> UpdateJoinRequestStatusAsync(string requestId, JoinRequestStatus status, CancellationToken ct = default)
            => inner.UpdateJoinRequestStatusAsync(requestId, status, ct);

        public Task<Result<IReadOnlyList<AcademyGrade>>> ListGradesAsync(string academyId, CancellationToken ct = default)
            => inner.ListGradesAsync(academyId, ct);

        public Task<Result<AcademyGrade>> CreateGradeAsync(AcademyGrade grade, CancellationToken ct = default)
            => inner.CreateGradeAsync(grade, ct);

        public Task<Result> DeleteGradeAsync(string gradeId, CancellationToken ct = default)
            => inner.DeleteGradeAsync(gradeId, ct);

        public Task<Result<IReadOnlyList<AcademyCourse>>> ListCoursesAsync(string gradeId, CancellationToken ct = default)
            => inner.ListCoursesAsync(gradeId, ct);

        public Task<Result<AcademyCourse>> GetCourseAsync(string courseId, CancellationToken ct = default)
            => inner.GetCourseAsync(courseId, ct);

        public Task<Result> SetCourseClassAsync(string courseId, string classId, string className, CancellationToken ct = default)
            => inner.SetCourseClassAsync(courseId, classId, className, ct);

        public Task<Result<AcademyCourse>> CreateCourseAsync(AcademyCourse course, CancellationToken ct = default)
            => inner.CreateCourseAsync(course, ct);

        public Task<Result> UpdateCourseAsync(AcademyCourse course, CancellationToken ct = default)
            => inner.UpdateCourseAsync(course, ct);

        public Task<Result> DeleteCourseAsync(string courseId, CancellationToken ct = default)
            => inner.DeleteCourseAsync(courseId, ct);

        public Task<Result<PaymentDetails>> GetPaymentDetailsAsync(string userId, CancellationToken ct = default)
            => inner.GetPaymentDetailsAsync(userId, ct);

        public Task<Result> UpsertPaymentDetailsAsync(PaymentDetails details, CancellationToken ct = default)
            => inner.UpsertPaymentDetailsAsync(details, ct);

        public Task<Result> CreateEnrollmentRequestAsync(CourseEnrollmentRequest request, CancellationToken ct = default)
            => inner.CreateEnrollmentRequestAsync(request, ct);

        public Task<Result> SubmitEnrollmentPaymentAsync(string requestId, string paymentReference, string paymentProofUrl, CancellationToken ct = default)
            => inner.SubmitEnrollmentPaymentAsync(requestId, paymentReference, paymentProofUrl, ct);

        public Task<Result> UpdateEnrollmentStatusAsync(string requestId, EnrollmentStatus status, CancellationToken ct = default)
            => inner.UpdateEnrollmentStatusAsync(requestId, status, ct);

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListEnrollmentRequestsForOwnerAsync(CancellationToken ct = default)
            => inner.ListEnrollmentRequestsForOwnerAsync(ct);

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListMyEnrollmentRequestsAsync(CancellationToken ct = default)
            => inner.ListMyEnrollmentRequestsAsync(ct);

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesAsync(string academyId, CancellationToken ct = default)
            => inner.ListClassesAsync(academyId, ct);

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesByGradeAsync(string gradeId, CancellationToken ct = default)
            => inner.ListClassesByGradeAsync(gradeId, ct);

        public Task<Result<AcademyClass>> GetClassAsync(string classId, CancellationToken ct = default)
            => inner.GetClassAsync(classId, ct);

        public Task<Result<AcademyClass>> CreateClassAsync(AcademyClass academyClass, CancellationToken ct = default)
            => inner.CreateClassAsync(academyClass, ct);

        public Task<Result> UpdateClassAsync(AcademyClass academyClass, CancellationToken ct = default)
            => inner.UpdateClassAsync(academyClass, ct);

        public Task<Result> DeleteClassAsync(string classId, CancellationToken ct = default)
            => inner.DeleteClassAsync(classId, ct);

        public Task<Result<IReadOnlyList<Module>>> ListModulesAsync(string classId, CancellationToken ct = default)
            => inner.ListModulesAsync(classId, ct);

        public Task<Result<IReadOnlyList<Lesson>>> ListLessonsAsync(string moduleId, CancellationToken ct = default)
            => inner.ListLessonsAsync(moduleId, ct);

        public Task<Result> EnrollAsync(Enrollment enrollment, CancellationToken ct = default)
            => inner.EnrollAsync(enrollment, ct);

        public Task<Result> UnenrollAsync(string studentId, string classId, CancellationToken ct = default)
            => inner.UnenrollAsync(studentId, classId, ct);

        public Task<Result<IReadOnlyList<Enrollment>>> ListEnrollmentsAsync(string studentId, CancellationToken ct = default)
            => inner.ListEnrollmentsAsync(studentId, ct);

        public Task<Result<IReadOnlyList<Enrollment>>> ListClassRosterAsync(string classId, CancellationToken ct = default)
            => inner.ListClassRosterAsync(classId, ct);

        public Task<Result<int>> CountEnrollmentsAsync(string classId, CancellationToken ct = default)
            => inner.CountEnrollmentsAsync(classId, ct);
    }
}
