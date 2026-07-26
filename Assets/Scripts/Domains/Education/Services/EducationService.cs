using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Education.Models;
using SReader.Domains.Education.Repositories;

namespace SReader.Domains.Education.Services
{
    public sealed class EducationService : IEducationService
    {
        readonly IEducationRepository repository;
        readonly CurrentSessionHolder session;
        readonly IClock clock;

        public EducationService(IEducationRepository repository, CurrentSessionHolder session, IClock clock)
        {
            this.repository = Guard.NotNull(repository, nameof(repository));
            this.session    = Guard.NotNull(session, nameof(session));
            this.clock      = Guard.NotNull(clock, nameof(clock));
        }

        public Task<Result<IReadOnlyList<Academy>>> ListAcademiesAsync(CancellationToken ct = default)
            => repository.ListAcademiesAsync(ct);

        public Task<Result<IReadOnlyList<Academy>>> ListMyAcademiesAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<Academy>>("Not signed in."));
            return repository.ListAcademiesByOwnerAsync(session.CurrentUserId, ct);
        }

        public Task<Result<Academy>> CreateAcademyAsync(string name, string description, string city, string country, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<Academy>("Not signed in."));
            if (string.IsNullOrWhiteSpace(name))
                return Task.FromResult(Result.Fail<Academy>("Please enter an academy name."));

            var academy = new Academy
            {
                OwnerId     = session.CurrentUserId,
                Name        = name.Trim(),
                Description = description?.Trim(),
                City        = city?.Trim(),
                Country     = country?.Trim(),
                CreatedAt   = clock.UtcNow
            };
            return repository.CreateAcademyAsync(academy, ct);
        }

        public Task<Result> UpdateAcademyAsync(Academy academy, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (academy == null || string.IsNullOrEmpty(academy.Id))
                return Task.FromResult(Result.Fail("An academy with an id is required."));
            // A tutor may only edit academies they own (RLS enforces this too).
            if (academy.OwnerId != session.CurrentUserId)
                return Task.FromResult(Result.Fail("You can only edit your own academies."));
            if (string.IsNullOrWhiteSpace(academy.Name))
                return Task.FromResult(Result.Fail("Please enter an academy name."));
            return repository.UpdateAcademyAsync(academy, ct);
        }

        public Task<Result> DeleteAcademyAsync(string academyId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(academyId))
                return Task.FromResult(Result.Fail("Academy id is required."));
            return repository.DeleteAcademyAsync(academyId, ct);
        }

        public Task<Result> RequestToJoinAsync(string academyId, string academyName, string studentName, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(academyId))
                return Task.FromResult(Result.Fail("Academy id is required."));

            return repository.CreateJoinRequestAsync(new AcademyJoinRequest
            {
                AcademyId   = academyId,
                AcademyName = academyName,
                StudentId   = session.CurrentUserId,
                StudentName = string.IsNullOrWhiteSpace(studentName) ? "A student" : studentName.Trim(),
                Status      = JoinRequestStatus.Pending,
                CreatedAt   = clock.UtcNow
            }, ct);
        }

        public Task<Result<IReadOnlyList<AcademyJoinRequest>>> ListMyAcademyRequestsAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<AcademyJoinRequest>>("Not signed in."));
            // RLS returns only requests for academies this tutor owns.
            return repository.ListJoinRequestsForOwnerAsync(ct);
        }

        public Task<Result> ApproveRequestAsync(string requestId, CancellationToken ct = default)
            => SetRequestStatus(requestId, JoinRequestStatus.Approved, ct);

        public Task<Result> RejectRequestAsync(string requestId, CancellationToken ct = default)
            => SetRequestStatus(requestId, JoinRequestStatus.Rejected, ct);

        Task<Result> SetRequestStatus(string requestId, JoinRequestStatus status, CancellationToken ct)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(requestId))
                return Task.FromResult(Result.Fail("Request id is required."));
            return repository.UpdateJoinRequestStatusAsync(requestId, status, ct);
        }

        // ── Grades & courses ──

        public Task<Result<IReadOnlyList<AcademyGrade>>> ListGradesAsync(string academyId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(academyId))
                return Task.FromResult(Result.Fail<IReadOnlyList<AcademyGrade>>("Academy id is required."));
            return repository.ListGradesAsync(academyId, ct);
        }

        public Task<Result<AcademyGrade>> AddGradeAsync(string academyId, EducationStage stage, string title, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<AcademyGrade>("Not signed in."));
            if (string.IsNullOrEmpty(academyId))
                return Task.FromResult(Result.Fail<AcademyGrade>("Academy id is required."));
            if (string.IsNullOrWhiteSpace(title))
                return Task.FromResult(Result.Fail<AcademyGrade>("Please enter the grade or program."));

            return repository.CreateGradeAsync(new AcademyGrade
            {
                AcademyId = academyId,
                Stage     = stage,
                Title     = title.Trim(),
                CreatedAt = clock.UtcNow
            }, ct);
        }

        public Task<Result> DeleteGradeAsync(string gradeId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(gradeId))
                return Task.FromResult(Result.Fail("Grade id is required."));
            return repository.DeleteGradeAsync(gradeId, ct);
        }

        public Task<Result<IReadOnlyList<AcademyCourse>>> ListCoursesAsync(string gradeId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(gradeId))
                return Task.FromResult(Result.Fail<IReadOnlyList<AcademyCourse>>("Grade id is required."));
            return repository.ListCoursesAsync(gradeId, ct);
        }

        public Task<Result<AcademyCourse>> AddCourseAsync(string gradeId, string name, string description, double price, string timeFrame, bool freeTrial, string deadline, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<AcademyCourse>("Not signed in."));
            if (string.IsNullOrEmpty(gradeId))
                return Task.FromResult(Result.Fail<AcademyCourse>("Grade id is required."));
            if (string.IsNullOrWhiteSpace(name))
                return Task.FromResult(Result.Fail<AcademyCourse>("Please enter a course name."));
            if (price < 0)
                return Task.FromResult(Result.Fail<AcademyCourse>("Price can't be negative."));

            return repository.CreateCourseAsync(new AcademyCourse
            {
                GradeId     = gradeId,
                Name        = name.Trim(),
                Description = description?.Trim(),
                Price       = price,
                TimeFrame   = timeFrame?.Trim(),
                FreeTrial   = freeTrial,
                Deadline    = deadline?.Trim(),
                CreatedAt   = clock.UtcNow
            }, ct);
        }

        public Task<Result> UpdateCourseAsync(AcademyCourse course, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (course == null || string.IsNullOrEmpty(course.Id))
                return Task.FromResult(Result.Fail("A course with an id is required."));
            if (string.IsNullOrWhiteSpace(course.Name))
                return Task.FromResult(Result.Fail("Please enter a course name."));
            if (course.Price < 0)
                return Task.FromResult(Result.Fail("Price can't be negative."));
            return repository.UpdateCourseAsync(course, ct);
        }

        public Task<Result> DeleteCourseAsync(string courseId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(courseId))
                return Task.FromResult(Result.Fail("Course id is required."));
            return repository.DeleteCourseAsync(courseId, ct);
        }

        public Task<Result> SetCourseClassAsync(string courseId, string classId, string className, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(courseId))
                return Task.FromResult(Result.Fail("Course id is required."));
            return repository.SetCourseClassAsync(courseId, classId, className, ct);
        }

        // ── Payment details ──

        public Task<Result<PaymentDetails>> GetMyPaymentDetailsAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<PaymentDetails>("Not signed in."));
            return repository.GetPaymentDetailsAsync(session.CurrentUserId, ct);
        }

        public Task<Result<PaymentDetails>> GetPaymentDetailsForAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(Result.Fail<PaymentDetails>("User id is required."));
            return repository.GetPaymentDetailsAsync(userId, ct);
        }

        public Task<Result> SavePaymentDetailsAsync(string accountName, string fnbAccount, string orangeMoney, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrWhiteSpace(fnbAccount) && string.IsNullOrWhiteSpace(orangeMoney))
                return Task.FromResult(Result.Fail("Add an FNB account or Orange Money number."));

            return repository.UpsertPaymentDetailsAsync(new PaymentDetails
            {
                UserId      = session.CurrentUserId,
                AccountName = accountName?.Trim(),
                FnbAccount  = fnbAccount?.Trim(),
                OrangeMoney = orangeMoney?.Trim()
            }, ct);
        }

        // ── Paid enrollment ──

        public Task<Result> RequestEnrollmentAsync(AcademyCourse course, Academy academy, string gradeTitle, string studentName, bool isFreeTrial, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (course == null || string.IsNullOrEmpty(course.Id) || academy == null)
                return Task.FromResult(Result.Fail("A course and academy are required."));
            if (!course.IsAvailable)
                return Task.FromResult(Result.Fail("This subject is no longer available — its deadline has passed."));
            if (isFreeTrial && !course.FreeTrial)
                return Task.FromResult(Result.Fail("This subject doesn't offer a free trial."));

            return repository.CreateEnrollmentRequestAsync(new CourseEnrollmentRequest
            {
                CourseId    = course.Id,
                CourseName  = course.Name,
                GradeTitle  = gradeTitle,
                AcademyId   = academy.Id,
                AcademyName = academy.Name,
                OwnerId     = academy.OwnerId,
                StudentId   = session.CurrentUserId,
                StudentName = string.IsNullOrWhiteSpace(studentName) ? "A student" : studentName.Trim(),
                Status      = EnrollmentStatus.Requested,
                IsFreeTrial = isFreeTrial,
                CreatedAt   = clock.UtcNow
            }, ct);
        }

        public Task<Result> SubmitEnrollmentPaymentAsync(string requestId, string paymentReference, string paymentProofUrl, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(requestId))
                return Task.FromResult(Result.Fail("Request id is required."));
            if (string.IsNullOrWhiteSpace(paymentProofUrl))
                return Task.FromResult(Result.Fail("Attach a proof-of-payment image first."));
            return repository.SubmitEnrollmentPaymentAsync(requestId, paymentReference?.Trim(), paymentProofUrl, ct);
        }

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListEnrollmentRequestsForOwnerAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<CourseEnrollmentRequest>>("Not signed in."));
            return repository.ListEnrollmentRequestsForOwnerAsync(ct);
        }

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListMyEnrollmentRequestsAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<CourseEnrollmentRequest>>("Not signed in."));
            return repository.ListMyEnrollmentRequestsAsync(ct);
        }

        public async Task<Result> ConfirmEnrollmentAsync(CourseEnrollmentRequest request, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Result.Fail("Not signed in.");
            if (request == null || string.IsNullOrEmpty(request.Id)) return Result.Fail("Request id is required.");

            var statusResult = await repository.UpdateEnrollmentStatusAsync(request.Id, EnrollmentStatus.Enrolled, ct);
            if (statusResult.IsFailure) return statusResult;

            // Place the now-confirmed student into the class that teaches this
            // subject, so they receive its assignments. Best-effort: a subject not
            // yet assigned to a class, or a transient enrol failure, does not undo
            // the confirmed enrolment (class_enrollments upsert is idempotent).
            if (!string.IsNullOrEmpty(request.CourseId))
            {
                var course = await repository.GetCourseAsync(request.CourseId, ct);
                if (course.IsSuccess && course.Value.HasClass)
                {
                    await repository.EnrollAsync(new Enrollment
                    {
                        StudentId      = request.StudentId,
                        StudentName    = request.StudentName,
                        ClassId        = course.Value.ClassId,
                        ClassName      = course.Value.ClassName,
                        GradeTitle     = request.GradeTitle,
                        AcademyId      = request.AcademyId,
                        AcademyName    = request.AcademyName,
                        EnrollmentDate = clock.UtcNow
                    }, ct);
                }
            }
            return Result.Ok();
        }

        public Task<Result> RejectEnrollmentAsync(string requestId, CancellationToken ct = default)
            => SetEnrollmentStatus(requestId, EnrollmentStatus.Rejected, ct);

        Task<Result> SetEnrollmentStatus(string requestId, EnrollmentStatus status, CancellationToken ct)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(requestId))
                return Task.FromResult(Result.Fail("Request id is required."));
            return repository.UpdateEnrollmentStatusAsync(requestId, status, ct);
        }

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesAsync(string academyId, CancellationToken ct = default)
            => repository.ListClassesAsync(academyId, ct);

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesForGradeAsync(string gradeId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(gradeId))
                return Task.FromResult(Result.Fail<IReadOnlyList<AcademyClass>>("Grade id is required."));
            return repository.ListClassesByGradeAsync(gradeId, ct);
        }

        public Task<Result<AcademyClass>> CreateClassAsync(AcademyClass template, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<AcademyClass>("Not signed in."));
            if (template == null)
                return Task.FromResult(Result.Fail<AcademyClass>("Class details are required."));
            if (string.IsNullOrEmpty(template.GradeId) || string.IsNullOrEmpty(template.AcademyId))
                return Task.FromResult(Result.Fail<AcademyClass>("A grade and academy are required."));
            if (string.IsNullOrWhiteSpace(template.Name))
                return Task.FromResult(Result.Fail<AcademyClass>("Please name the class."));
            if (template.MaxStudents < 0)
                return Task.FromResult(Result.Fail<AcademyClass>("Capacity can't be negative."));

            template.OwnerId     = session.CurrentUserId;   // the academy owner (tutor)
            template.Name        = template.Name.Trim();
            template.Description = template.Description?.Trim();
            template.CreatedAt   = clock.UtcNow;
            return repository.CreateClassAsync(template, ct);
        }

        public Task<Result> UpdateClassAsync(AcademyClass academyClass, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (academyClass == null || string.IsNullOrEmpty(academyClass.Id))
                return Task.FromResult(Result.Fail("A class with an id is required."));
            if (string.IsNullOrWhiteSpace(academyClass.Name))
                return Task.FromResult(Result.Fail("Please name the class."));
            if (academyClass.MaxStudents < 0)
                return Task.FromResult(Result.Fail("Capacity can't be negative."));

            academyClass.Name        = academyClass.Name.Trim();
            academyClass.Description = academyClass.Description?.Trim();
            return repository.UpdateClassAsync(academyClass, ct);
        }

        public Task<Result> DeleteClassAsync(string classId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(classId))
                return Task.FromResult(Result.Fail("Class id is required."));
            return repository.DeleteClassAsync(classId, ct);
        }

        public Task<Result<int>> CountClassStudentsAsync(string classId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(classId))
                return Task.FromResult(Result.Fail<int>("Class id is required."));
            return repository.CountEnrollmentsAsync(classId, ct);
        }

        public Task<Result<IReadOnlyList<Module>>> ListModulesAsync(string classId, CancellationToken ct = default)
            => repository.ListModulesAsync(classId, ct);

        public Task<Result<IReadOnlyList<Lesson>>> ListLessonsAsync(string moduleId, CancellationToken ct = default)
            => repository.ListLessonsAsync(moduleId, ct);

        public async Task<Result> EnrollStudentAsync(string studentId, string classId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(classId))
                return Result.Fail("Student and class ids are required.");

            var classResult = await repository.GetClassAsync(classId, ct);
            if (classResult.IsFailure) return classResult;

            var capacity = await CheckCapacityAsync(classResult.Value, ct);
            if (capacity.IsFailure) return capacity;

            return await repository.EnrollAsync(EnrollmentFor(classResult.Value, studentId, null), ct);
        }

        public async Task<Result> EnrollInClassAsync(AcademyClass academyClass, string studentName, CancellationToken ct = default)
        {
            if (!session.IsSignedIn) return Result.Fail("Not signed in.");
            if (academyClass == null || string.IsNullOrEmpty(academyClass.Id))
                return Result.Fail("A class is required.");

            var capacity = await CheckCapacityAsync(academyClass, ct);
            if (capacity.IsFailure) return capacity;

            var name = string.IsNullOrWhiteSpace(studentName) ? "A student" : studentName.Trim();
            return await repository.EnrollAsync(EnrollmentFor(academyClass, session.CurrentUserId, name), ct);
        }

        public Task<Result> UnenrollStudentAsync(string studentId, string classId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(classId))
                return Task.FromResult(Result.Fail("Student and class ids are required."));
            return repository.UnenrollAsync(studentId, classId, ct);
        }

        public Task<Result> LeaveClassAsync(string classId, CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(classId))
                return Task.FromResult(Result.Fail("Class id is required."));
            return repository.UnenrollAsync(session.CurrentUserId, classId, ct);
        }

        public Task<Result<IReadOnlyList<Enrollment>>> ListEnrollmentsAsync(string studentId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(studentId))
                return Task.FromResult(Result.Fail<IReadOnlyList<Enrollment>>("Student id is required."));
            return repository.ListEnrollmentsAsync(studentId, ct);
        }

        public Task<Result<IReadOnlyList<Enrollment>>> ListMyClassesAsync(CancellationToken ct = default)
        {
            if (!session.IsSignedIn)
                return Task.FromResult(Result.Fail<IReadOnlyList<Enrollment>>("Not signed in."));
            return repository.ListEnrollmentsAsync(session.CurrentUserId, ct);
        }

        public Task<Result<IReadOnlyList<Enrollment>>> ListClassRosterAsync(string classId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(classId))
                return Task.FromResult(Result.Fail<IReadOnlyList<Enrollment>>("Class id is required."));
            return repository.ListClassRosterAsync(classId, ct);
        }

        // A class may not exceed its MaxStudents capacity (0 = unlimited).
        async Task<Result> CheckCapacityAsync(AcademyClass cls, CancellationToken ct)
        {
            if (cls == null || cls.MaxStudents <= 0) return Result.Ok();
            var countResult = await repository.CountEnrollmentsAsync(cls.Id, ct);
            if (countResult.IsFailure) return Result.Fail(countResult.Error);
            return countResult.Value >= cls.MaxStudents ? Result.Fail("This class is full.") : Result.Ok();
        }

        Enrollment EnrollmentFor(AcademyClass cls, string studentId, string studentName) => new Enrollment
        {
            StudentId      = studentId,
            StudentName    = studentName,
            ClassId        = cls.Id,
            ClassName      = cls.Name,
            GradeTitle     = cls.GradeTitle,
            AcademyId      = cls.AcademyId,
            AcademyName    = cls.AcademyName,
            EnrollmentDate = clock.UtcNow
        };
    }
}
