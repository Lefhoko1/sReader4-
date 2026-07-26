using System.Collections.Generic;

namespace SReader.Domains.Social.Models
{
    /// <summary>
    /// One enrolled subject of a student, as seen on their public profile:
    /// which academy, grade and subject, and the tutor (staff) teaching it.
    /// Sourced from the friend-safe <c>student_academics</c> view — confirmed
    /// enrolments only, no payment data.
    /// </summary>
    public sealed class StudentSubject
    {
        public string AcademyId { get; set; }
        public string AcademyName { get; set; }
        public string GradeTitle { get; set; }
        public string CourseName { get; set; }   // the subject / module
        public string TutorId { get; set; }
        public string TutorName { get; set; }
    }

    /// <summary>
    /// A student's enrolment at one academy/grade, grouped for display: the
    /// academy, the grade they're in, the tutor/staff, and every subject they
    /// take there. Built by the service from the raw <see cref="StudentSubject"/>
    /// rows so the profile screen can show "Academy → grade → subjects + staff".
    /// </summary>
    public sealed class StudentAcademicRecord
    {
        public string AcademyId { get; set; }
        public string AcademyName { get; set; }
        public string GradeTitle { get; set; }
        public string TutorName { get; set; }
        public List<string> Subjects { get; } = new List<string>();
    }
}
