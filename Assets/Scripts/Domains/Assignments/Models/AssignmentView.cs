namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// An assignment as the signed-in student sees it: the assignment plus their
    /// own attempt status and schedule, so a list can show a status badge and a
    /// "due soon / overdue" hint without extra per-row lookups. Built by the
    /// service by joining assignments to the student's attempts and schedules.
    /// </summary>
    public sealed class AssignmentView
    {
        public Assignment Assignment { get; set; }

        /// <summary>The student's attempt status; NotStarted if they haven't begun.</summary>
        public AttemptStatus Status { get; set; } = AttemptStatus.NotStarted;

        /// <summary>The student's own schedule for this assignment, if any.</summary>
        public AssignmentSchedule MySchedule { get; set; }

        public bool Submitted => Status == AttemptStatus.Submitted || Status == AttemptStatus.Graded;
        public bool Completed => Status == AttemptStatus.Completed || Submitted;
        public bool Started   => Status != AttemptStatus.NotStarted;
        public bool IsScheduled => MySchedule != null;

        /// <summary>Short status label for a badge.</summary>
        public string StatusLabel
        {
            get
            {
                switch (Status)
                {
                    case AttemptStatus.Graded:     return "Graded";
                    case AttemptStatus.Submitted:  return "Submitted";
                    case AttemptStatus.Completed:  return "Completed";
                    case AttemptStatus.InProgress: return "In progress";
                    default:                       return Assignment != null && Assignment.IsOverdue ? "Overdue" : "Not started";
                }
            }
        }
    }
}
