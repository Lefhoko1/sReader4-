using SReader.Core.Events;

namespace SReader.Domains.Assignments.Events
{
    /// <summary>Raised when a student submits — Notifications reacts without coupling.</summary>
    public sealed class AssignmentSubmittedEvent : IAppEvent
    {
        public string AssignmentId { get; }
        public string StudentId { get; }
        public string SubmissionId { get; }

        public AssignmentSubmittedEvent(string assignmentId, string studentId, string submissionId)
        {
            AssignmentId = assignmentId;
            StudentId = studentId;
            SubmissionId = submissionId;
        }
    }

    public sealed class AssignmentGradedEvent : IAppEvent
    {
        public string SubmissionId { get; }
        public int Grade { get; }

        public AssignmentGradedEvent(string submissionId, int grade)
        {
            SubmissionId = submissionId;
            Grade = grade;
        }
    }
}
