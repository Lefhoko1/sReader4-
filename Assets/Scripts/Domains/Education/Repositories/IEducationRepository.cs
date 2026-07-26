using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Education.Models;

namespace SReader.Domains.Education.Repositories
{
    public interface IEducationRepository
    {
        Task<Result<IReadOnlyList<Academy>>> ListAcademiesAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<Academy>>> ListAcademiesByOwnerAsync(string ownerId, CancellationToken ct = default);
        Task<Result<Academy>> GetAcademyAsync(string academyId, CancellationToken ct = default);
        Task<Result<Academy>> CreateAcademyAsync(Academy academy, CancellationToken ct = default);
        Task<Result> UpdateAcademyAsync(Academy academy, CancellationToken ct = default);
        Task<Result> DeleteAcademyAsync(string academyId, CancellationToken ct = default);

        // Join requests (students ask, the owning tutor approves/rejects).
        Task<Result> CreateJoinRequestAsync(AcademyJoinRequest request, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AcademyJoinRequest>>> ListJoinRequestsForOwnerAsync(CancellationToken ct = default);
        Task<Result> UpdateJoinRequestStatusAsync(string requestId, JoinRequestStatus status, CancellationToken ct = default);

        // Grades (PSLE / JC / BGCSE / university programs) an academy offers.
        Task<Result<IReadOnlyList<AcademyGrade>>> ListGradesAsync(string academyId, CancellationToken ct = default);
        Task<Result<AcademyGrade>> CreateGradeAsync(AcademyGrade grade, CancellationToken ct = default);
        Task<Result> DeleteGradeAsync(string gradeId, CancellationToken ct = default);

        // Courses/subjects/modules under a grade.
        Task<Result<IReadOnlyList<AcademyCourse>>> ListCoursesAsync(string gradeId, CancellationToken ct = default);
        Task<Result<AcademyCourse>> GetCourseAsync(string courseId, CancellationToken ct = default);
        Task<Result<AcademyCourse>> CreateCourseAsync(AcademyCourse course, CancellationToken ct = default);
        /// <summary>Set (or clear, with null) the class a subject is taught in.</summary>
        Task<Result> SetCourseClassAsync(string courseId, string classId, string className, CancellationToken ct = default);
        Task<Result> UpdateCourseAsync(AcademyCourse course, CancellationToken ct = default);
        Task<Result> DeleteCourseAsync(string courseId, CancellationToken ct = default);

        // Tutor payment details (single source of truth, keyed by user id).
        Task<Result<PaymentDetails>> GetPaymentDetailsAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertPaymentDetailsAsync(PaymentDetails details, CancellationToken ct = default);

        // Paid enrollment requests (student requests a module/subject, pays, enrolls).
        Task<Result> CreateEnrollmentRequestAsync(CourseEnrollmentRequest request, CancellationToken ct = default);
        Task<Result> SubmitEnrollmentPaymentAsync(string requestId, string paymentReference, string paymentProofUrl, CancellationToken ct = default);
        Task<Result> UpdateEnrollmentStatusAsync(string requestId, EnrollmentStatus status, CancellationToken ct = default);
        Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListEnrollmentRequestsForOwnerAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListMyEnrollmentRequestsAsync(CancellationToken ct = default);

        Task<Result<IReadOnlyList<AcademyClass>>> ListClassesAsync(string academyId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AcademyClass>>> ListClassesByGradeAsync(string gradeId, CancellationToken ct = default);
        Task<Result<AcademyClass>> GetClassAsync(string classId, CancellationToken ct = default);
        Task<Result<AcademyClass>> CreateClassAsync(AcademyClass academyClass, CancellationToken ct = default);
        Task<Result> UpdateClassAsync(AcademyClass academyClass, CancellationToken ct = default);
        Task<Result> DeleteClassAsync(string classId, CancellationToken ct = default);

        Task<Result<IReadOnlyList<Module>>> ListModulesAsync(string classId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Lesson>>> ListLessonsAsync(string moduleId, CancellationToken ct = default);

        Task<Result> EnrollAsync(Enrollment enrollment, CancellationToken ct = default);
        Task<Result> UnenrollAsync(string studentId, string classId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Enrollment>>> ListEnrollmentsAsync(string studentId, CancellationToken ct = default);
        /// <summary>The students enrolled in a class (the tutor's roster).</summary>
        Task<Result<IReadOnlyList<Enrollment>>> ListClassRosterAsync(string classId, CancellationToken ct = default);
        Task<Result<int>> CountEnrollmentsAsync(string classId, CancellationToken ct = default);
    }
}
