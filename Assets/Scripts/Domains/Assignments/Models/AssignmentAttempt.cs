using System;
using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    public enum AttemptStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Submitted,
        Graded
    }

    public class AssignmentAttempt : Entity
    {
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public AttemptStatus Status { get; set; }
    }
}
