using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Education.Models;

namespace SReader.Domains.Education.Services
{
    public interface IEducationService
    {
        // ── Academies (owned by tutors) ──
        Task<Result<IReadOnlyList<Academy>>> ListAcademiesAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<Academy>>> ListMyAcademiesAsync(CancellationToken ct = default);
        Task<Result<Academy>> CreateAcademyAsync(string name, string description, string city, string country, CancellationToken ct = default);
        Task<Result> UpdateAcademyAsync(Academy academy, CancellationToken ct = default);
        Task<Result> DeleteAcademyAsync(string academyId, CancellationToken ct = default);

        // ── Join requests ──
        Task<Result> RequestToJoinAsync(string academyId, string academyName, string studentName, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AcademyJoinRequest>>> ListMyAcademyRequestsAsync(CancellationToken ct = default);
        Task<Result> ApproveRequestAsync(string requestId, CancellationToken ct = default);
        Task<Result> RejectRequestAsync(string requestId, CancellationToken ct = default);

        // ── Grades & courses (education structure) ──
        Task<Result<IReadOnlyList<AcademyGrade>>> ListGradesAsync(string academyId, CancellationToken ct = default);
        Task<Result<AcademyGrade>> AddGradeAsync(string academyId, EducationStage stage, string title, CancellationToken ct = default);
        Task<Result> DeleteGradeAsync(string gradeId, CancellationToken ct = default);

        Task<Result<IReadOnlyList<AcademyCourse>>> ListCoursesAsync(string gradeId, CancellationToken ct = default);
        Task<Result<AcademyCourse>> AddCourseAsync(string gradeId, string name, string description, double price, string timeFrame, bool freeTrial, string deadline, CancellationToken ct = default);
        Task<Result> UpdateCourseAsync(AcademyCourse course, CancellationToken ct = default);
        Task<Result> DeleteCourseAsync(string courseId, CancellationToken ct = default);
        /// <summary>Assign a subject to a class (or clear it with a null classId).</summary>
        Task<Result> SetCourseClassAsync(string courseId, string classId, string className, CancellationToken ct = default);

        // ── Tutor payment details (single source of truth) ──
        Task<Result<PaymentDetails>> GetMyPaymentDetailsAsync(CancellationToken ct = default);
        Task<Result<PaymentDetails>> GetPaymentDetailsForAsync(string userId, CancellationToken ct = default);
        Task<Result> SavePaymentDetailsAsync(string accountName, string fnbAccount, string orangeMoney, CancellationToken ct = default);

        // ── Paid enrollment in a module/subject ──
        Task<Result> RequestEnrollmentAsync(AcademyCourse course, Academy academy, string gradeTitle, string studentName, bool isFreeTrial, CancellationToken ct = default);
        Task<Result> SubmitEnrollmentPaymentAsync(string requestId, string paymentReference, string paymentProofUrl, CancellationToken ct = default);
        Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListEnrollmentRequestsForOwnerAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListMyEnrollmentRequestsAsync(CancellationToken ct = default);
        /// <summary>Confirm a paid request: marks it Enrolled AND enrols the student into the subject's class (if one is set).</summary>
        Task<Result> ConfirmEnrollmentAsync(CourseEnrollmentRequest request, CancellationToken ct = default);
        Task<Result> RejectEnrollmentAsync(string requestId, CancellationToken ct = default);

        // ── Classes (under a grade) ──
        Task<Result<IReadOnlyList<AcademyClass>>> ListClassesAsync(string academyId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AcademyClass>>> ListClassesForGradeAsync(string gradeId, CancellationToken ct = default);
        Task<Result<AcademyClass>> CreateClassAsync(AcademyClass template, CancellationToken ct = default);
        Task<Result> UpdateClassAsync(AcademyClass academyClass, CancellationToken ct = default);
        Task<Result> DeleteClassAsync(string classId, CancellationToken ct = default);
        Task<Result<int>> CountClassStudentsAsync(string classId, CancellationToken ct = default);

        Task<Result<IReadOnlyList<Module>>> ListModulesAsync(string classId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Lesson>>> ListLessonsAsync(string moduleId, CancellationToken ct = default);

        // ── Class enrolment ──
        Task<Result> EnrollStudentAsync(string studentId, string classId, CancellationToken ct = default);
        /// <summary>Self-enrol the signed-in student into a class (capacity-checked).</summary>
        Task<Result> EnrollInClassAsync(AcademyClass academyClass, string studentName, CancellationToken ct = default);
        Task<Result> UnenrollStudentAsync(string studentId, string classId, CancellationToken ct = default);
        /// <summary>Leave a class (signed-in student).</summary>
        Task<Result> LeaveClassAsync(string classId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Enrollment>>> ListEnrollmentsAsync(string studentId, CancellationToken ct = default);
        /// <summary>The classes the signed-in student is enrolled in.</summary>
        Task<Result<IReadOnlyList<Enrollment>>> ListMyClassesAsync(CancellationToken ct = default);
        /// <summary>The students enrolled in a class (the tutor's roster).</summary>
        Task<Result<IReadOnlyList<Enrollment>>> ListClassRosterAsync(string classId, CancellationToken ct = default);
    }
}
