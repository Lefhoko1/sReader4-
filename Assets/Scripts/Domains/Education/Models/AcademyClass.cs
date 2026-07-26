using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A class within an academy, belonging to a grade/standard — e.g. "Form 2
    /// Blue" under the "BGCSE" grade. Students enrol into a class, and a tutor
    /// assigns assignments to it. The owner is the academy owner (the tutor).
    /// AcademyName/GradeTitle are denormalised so a class card reads on its own.
    /// </summary>
    public class AcademyClass : Entity
    {
        public string AcademyId { get; set; }
        public string GradeId { get; set; }
        public string OwnerId { get; set; }

        public string AcademyName { get; set; }
        public string GradeTitle { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>0 = no cap; otherwise enrolment is refused once full.</summary>
        public int MaxStudents { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CapacityLabel => MaxStudents > 0 ? $"Up to {MaxStudents} students" : "Open enrolment";
    }
}
