using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;

namespace SReader.Domains.Assignments.Services
{
    public interface IAssignmentService
    {
        Task<Result<IReadOnlyList<Assignment>>> ListForClassAsync(string classId, CancellationToken ct = default);
        Task<Result<Assignment>> GetAsync(string assignmentId, CancellationToken ct = default);

        /// <summary>Assign a new assignment to a class (tutor). Returns the created assignment.</summary>
        Task<Result<Assignment>> CreateForClassAsync(string classId, string title, string instructions,
            System.DateTime dueDate, int maxScore, CancellationToken ct = default);

        /// <summary>Edit an existing assignment (tutor).</summary>
        Task<Result> UpdateAsync(Assignment assignment, string title, string instructions,
            System.DateTime dueDate, int maxScore, CancellationToken ct = default);

        /// <summary>Remove an assignment (tutor).</summary>
        Task<Result> DeleteAsync(string assignmentId, CancellationToken ct = default);

        Task<Result> StartAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default);
        Task<Result> CompleteAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default);

        /// <summary>
        /// Reset the student's attempt back to the start so they can play the
        /// assignment again, even after submitting it. Clears the attempt status
        /// and timestamps; existing submissions (the tutor's record) are kept.
        /// </summary>
        Task<Result> ResetAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default);
        Task<Result> SubmitAsync(string assignmentId, string studentId, string filePath, CancellationToken ct = default);
        Task<Result> GradeAsync(string submissionId, int grade, string feedback, CancellationToken ct = default);

        /// <summary>Write the structured content payload (tutor; schema lands later).</summary>
        Task<Result> SetContentAsync(string assignmentId, string contentJson, CancellationToken ct = default);

        // ── Student-centric (signed-in student) ──
        /// <summary>Every attempt the signed-in student has — to badge their assignment lists.</summary>
        Task<Result<IReadOnlyList<AssignmentAttempt>>> ListMyAttemptsAsync(CancellationToken ct = default);

        // ── Schedules ──
        /// <summary>Create/replace the signed-in student's schedule for an assignment.</summary>
        Task<Result> ScheduleAsync(Assignment assignment, System.DateTime when, string note, string studentName, bool hidden, CancellationToken ct = default);
        Task<Result> UnscheduleAsync(string scheduleId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AssignmentSchedule>>> ListMySchedulesAsync(CancellationToken ct = default);
        /// <summary>Schedules for one assignment the student is allowed to see (theirs + visible classmates/friends).</summary>
        Task<Result<IReadOnlyList<AssignmentSchedule>>> ListSchedulesForAssignmentAsync(string assignmentId, CancellationToken ct = default);
    }
}
