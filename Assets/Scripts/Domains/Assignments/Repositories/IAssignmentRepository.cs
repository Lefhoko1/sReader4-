using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Repositories
{
    public interface IAssignmentRepository
    {
        Task<Result<Assignment>> GetAsync(string assignmentId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Assignment>>> ListByClassAsync(string classId, CancellationToken ct = default);
        Task<Result> CreateAsync(Assignment assignment, CancellationToken ct = default);
        Task<Result> UpdateAsync(Assignment assignment, CancellationToken ct = default);
        Task<Result> DeleteAsync(string assignmentId, CancellationToken ct = default);

        Task<Result<IReadOnlyList<AssignmentFile>>> ListFilesAsync(string assignmentId, CancellationToken ct = default);

        Task<Result<AssignmentAttempt>> GetAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default);
        Task<Result> UpsertAttemptAsync(AssignmentAttempt attempt, CancellationToken ct = default);

        Task<Result> CreateSubmissionAsync(AssignmentSubmission submission, CancellationToken ct = default);
        Task<Result<AssignmentSubmission>> GetSubmissionAsync(string attemptId, CancellationToken ct = default);
        Task<Result> GradeSubmissionAsync(string submissionId, int grade, string feedback, CancellationToken ct = default);

        /// <summary>Every attempt the student has, in any status (to badge a list).</summary>
        Task<Result<IReadOnlyList<AssignmentAttempt>>> ListAttemptsForStudentAsync(string studentId, CancellationToken ct = default);

        /// <summary>Write the structured content payload (kept out of the general update).</summary>
        Task<Result> SetContentAsync(string assignmentId, string contentJson, CancellationToken ct = default);

        // ── Schedules (a student's plan; visible to classmates/friends unless hidden) ──
        Task<Result> UpsertScheduleAsync(AssignmentSchedule schedule, CancellationToken ct = default);
        Task<Result> DeleteScheduleAsync(string scheduleId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AssignmentSchedule>>> ListMySchedulesAsync(string studentId, CancellationToken ct = default);
        /// <summary>Schedules for one assignment the viewer is allowed to see (RLS-filtered).</summary>
        Task<Result<IReadOnlyList<AssignmentSchedule>>> ListSchedulesForAssignmentAsync(string assignmentId, CancellationToken ct = default);
    }
}
