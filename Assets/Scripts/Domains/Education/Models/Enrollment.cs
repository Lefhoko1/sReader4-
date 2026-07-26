using System;
using SReader.Core.Common;

namespace SReader.Domains.Education.Models
{
    /// <summary>
    /// A student's enrolment in a class. Denormalised class/grade/academy names
    /// so a student's "My classes" list and a tutor's roster render without
    /// extra lookups.
    /// </summary>
    public class Enrollment : Entity
    {
        public string StudentId { get; set; }
        public string ClassId { get; set; }

        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string GradeTitle { get; set; }
        public string AcademyId { get; set; }
        public string AcademyName { get; set; }

        public DateTime EnrollmentDate { get; set; }
    }
}
