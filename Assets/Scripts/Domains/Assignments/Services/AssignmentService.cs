using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Events;
using SReader.Core.Logging;
using SReader.Domains.Assignments.Events;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Repositories;

namespace SReader.Domains.Assignments.Services
{
    public sealed class AssignmentService : IAssignmentService
    {
        readonly IAssignmentRepository repository;
        readonly CurrentSessionHolder session;
        readonly IEventBus eventBus;
        readonly IAppLogger logger;
        readonly IClock clock;

        public AssignmentService(IAssignmentRepository repository, CurrentSessionHolder session,
            IEventBus eventBus, IAppLogger logger, IClock clock)
        {
            this.repository = Guard.NotNull(repository, nameof(repository));
            this.session    = Guard.NotNull(session, nameof(session));
            this.eventBus   = Guard.NotNull(eventBus, nameof(eventBus));
            this.logger     = Guard.NotNull(logger, nameof(logger));
            this.clock      = Guard.NotNull(clock, nameof(clock));
        }

        public Task<Result<IReadOnlyList<Assignment>>> ListForClassAsync(string classId, CancellationToken ct = default)
            => repository.ListByClassAsync(classId, ct);

        public Task<Result<Assignment>> GetAsync(string assignmentId, CancellationToken ct = default)
            => repository.GetAsync(assignmentId, ct);

        public async Task<Result<Assignment>> CreateForClassAsync(string classId, string title, string instructions,
            System.DateTime dueDate, int maxScore, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(classId)) return Result.Fail<Assignment>("A class is required.");
            if (string.IsNullOrWhiteSpace(title)) return Result.Fail<Assignment>("Please enter a title.");
            if (maxScore < 0) return Result.Fail<Assignment>("Max score can't be negative.");

            var now = clock.UtcNow;
            var assignment = new Assignment
            {
                ClassId      = classId,
                Title        = title.Trim(),
                Instructions = instructions?.Trim(),
                DueDate      = dueDate,
                MaxScore     = maxScore,
                Version      = 1,
                CreatedAt    = now,
                UpdatedAt    = now
            };

            var created = await repository.CreateAsync(assignment, ct);
            if (created.IsFailure) return Result.Fail<Assignment>(created.Error);

            logger.Info($"Assignment '{assignment.Title}' assigned to class {classId}");
            return Result.Ok(assignment);
        }

        public Task<Result> UpdateAsync(Assignment assignment, string title, string instructions,
            System.DateTime dueDate, int maxScore, CancellationToken ct = default)
        {
            if (assignment == null || string.IsNullOrEmpty(assignment.Id))
                return Task.FromResult(Result.Fail("An assignment with an id is required."));
            if (string.IsNullOrWhiteSpace(title)) return Task.FromResult(Result.Fail("Please enter a title."));
            if (maxScore < 0) return Task.FromResult(Result.Fail("Max score can't be negative."));

            assignment.Title        = title.Trim();
            assignment.Instructions = instructions?.Trim();
            assignment.DueDate      = dueDate;
            assignment.MaxScore     = maxScore;
            assignment.Version      = assignment.Version <= 0 ? 1 : assignment.Version + 1;
            assignment.UpdatedAt    = clock.UtcNow;
            return repository.UpdateAsync(assignment, ct);
        }

        public Task<Result> DeleteAsync(string assignmentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId))
                return Task.FromResult(Result.Fail("Assignment id is required."));
            return repository.DeleteAsync(assignmentId, ct);
        }

        public async Task<Result> StartAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default)
        {
            var existing = await repository.GetAttemptAsync(assignmentId, studentId, ct);
            var attempt = existing.IsSuccess && existing.Value != null
                ? existing.Value
                : new AssignmentAttempt { AssignmentId = assignmentId, StudentId = studentId };

            if (attempt.Status == AttemptStatus.Submitted || attempt.Status == AttemptStatus.Graded)
                return Result.Fail("This assignment has already been submitted.");

            attempt.Status = AttemptStatus.InProgress;
            attempt.StartedAt = attempt.StartedAt ?? clock.UtcNow;
            return await repository.UpsertAttemptAsync(attempt, ct);
        }

        public async Task<Result> CompleteAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default)
        {
            var existing = await repository.GetAttemptAsync(assignmentId, studentId, ct);
            if (existing.IsFailure || existing.Value == null)
                return Result.Fail("No attempt in progress for this assignment.");

            var attempt = existing.Value;
            attempt.Status = AttemptStatus.Completed;
            attempt.CompletedAt = clock.UtcNow;
            return await repository.UpsertAttemptAsync(attempt, ct);
        }

        public Task<Result> ResetAttemptAsync(string assignmentId, string studentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId)) return Task.FromResult(Result.Fail("Assignment id is required."));
            if (string.IsNullOrEmpty(studentId)) return Task.FromResult(Result.Fail("A student is required."));

            // Upsert (keyed on assignment+student) back to a clean slate so the
            // student can play again. Submissions are left untouched.
            return repository.UpsertAttemptAsync(new AssignmentAttempt
            {
                AssignmentId = assignmentId,
                StudentId    = studentId,
                Status       = AttemptStatus.NotStarted,
                StartedAt    = null,
                CompletedAt  = null
            }, ct);
        }

        public async Task<Result> SubmitAsync(string assignmentId, string studentId, string filePath, CancellationToken ct = default)
        {
            var existing = await repository.GetAttemptAsync(assignmentId, studentId, ct);
            if (existing.IsFailure || existing.Value == null)
                return Result.Fail("Start the assignment before submitting.");

            var attempt = existing.Value;
            if (attempt.Status == AttemptStatus.Submitted || attempt.Status == AttemptStatus.Graded)
                return Result.Fail("This assignment has already been submitted.");

            var submission = new AssignmentSubmission
            {
                AttemptId = attempt.Id,
                AssignmentId = assignmentId,
                StudentId = studentId,
                SubmittedAt = clock.UtcNow,
                FilePath = filePath
            };

            var created = await repository.CreateSubmissionAsync(submission, ct);
            if (created.IsFailure) return created;

            attempt.Status = AttemptStatus.Submitted;
            var updated = await repository.UpsertAttemptAsync(attempt, ct);
            if (updated.IsFailure) return updated;

            eventBus.Publish(new AssignmentSubmittedEvent(assignmentId, studentId, submission.Id));
            logger.Info($"Assignment {assignmentId} submitted by {studentId}");
            return Result.Ok();
        }

        public async Task<Result> GradeAsync(string submissionId, int grade, string feedback, CancellationToken ct = default)
        {
            if (grade < 0) return Result.Fail("Grade cannot be negative.");

            var result = await repository.GradeSubmissionAsync(submissionId, grade, feedback, ct);
            if (result.IsFailure) return result;

            eventBus.Publish(new AssignmentGradedEvent(submissionId, grade));
            return Result.Ok();
        }

        public Task<Result> SetContentAsync(string assignmentId, string contentJson, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId)) return Task.FromResult(Result.Fail("Assignment id is required."));
            return repository.SetContentAsync(assignmentId, contentJson, ct);
        }

        public Task<Result<IReadOnlyList<AssignmentAttempt>>> ListMyAttemptsAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<AssignmentAttempt>>("Not signed in."));
            return repository.ListAttemptsForStudentAsync(session.CurrentUserId, ct);
        }

        // ── Schedules ──

        public Task<Result> ScheduleAsync(Assignment assignment, DateTime when, string note, string studentName, bool hidden, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (assignment == null || string.IsNullOrEmpty(assignment.Id))
                return Task.FromResult(Result.Fail("An assignment is required."));

            var now = clock.UtcNow;
            return repository.UpsertScheduleAsync(new AssignmentSchedule
            {
                AssignmentId    = assignment.Id,
                StudentId       = session.CurrentUserId,
                StudentName     = string.IsNullOrWhiteSpace(studentName) ? "A student" : studentName.Trim(),
                ClassId         = assignment.ClassId,
                AssignmentTitle = assignment.Title,
                ScheduledFor    = when,
                Note            = note?.Trim(),
                Hidden          = hidden,
                CreatedAt       = now,
                UpdatedAt       = now
            }, ct);
        }

        public Task<Result> UnscheduleAsync(string scheduleId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(scheduleId)) return Task.FromResult(Result.Fail("Schedule id is required."));
            return repository.DeleteScheduleAsync(scheduleId, ct);
        }

        public Task<Result<IReadOnlyList<AssignmentSchedule>>> ListMySchedulesAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<AssignmentSchedule>>("Not signed in."));
            return repository.ListMySchedulesAsync(session.CurrentUserId, ct);
        }

        public Task<Result<IReadOnlyList<AssignmentSchedule>>> ListSchedulesForAssignmentAsync(string assignmentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assignmentId))
                return Task.FromResult(Result.Fail<IReadOnlyList<AssignmentSchedule>>("Assignment id is required."));
            return repository.ListSchedulesForAssignmentAsync(assignmentId, ct);
        }
    }
}
