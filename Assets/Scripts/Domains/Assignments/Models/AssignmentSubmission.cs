using System;
using SReader.Core.Common;

namespace SReader.Domains.Assignments.Models
{
    public class AssignmentSubmission : Entity
    {
        public string AttemptId { get; set; }
        // Denormalised so RLS (student_id = auth.uid()) and tutor queries are simple.
        public string AssignmentId { get; set; }
        public string StudentId { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string FilePath { get; set; }

        public int? Grade { get; set; }
        public string Feedback { get; set; }
    }
}
