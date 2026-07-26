using System;
using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    /// <summary>
    /// A student's plan to do an assignment at a chosen time. Saved both online
    /// (Supabase) and offline (SQLite mirror). Unless <see cref="Hidden"/>, it is
    /// visible to the student's classmates (same class) and friends so they can
    /// plan to do the work together. Denormalised title/class so a schedule card
    /// reads on its own.
    /// </summary>
    public class AssignmentSchedule : Entity
    {
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }

        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public string AssignmentTitle { get; set; }

        public DateTime ScheduledFor { get; set; }
        public string Note { get; set; }

        /// <summary>When true, classmates and friends can't see this schedule.</summary>
        public bool Hidden { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
