using System.Collections.Generic;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Education.Models;
using SReader.Domains.Education.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>
    /// Backs the Academies CRUD on the tutor home (and the student "request to
    /// join"). Holds the loaded lists and forwards create/edit/delete/approve to
    /// IEducationService. Null service = placeholder mode (no AppCompositionRoot).
    /// </summary>
    public sealed class AcademyViewModel : ViewModelBase
    {
        readonly IEducationService education;
        readonly CurrentSessionHolder session;
        readonly IImagePicker imagePicker;
        readonly IPaymentProofUploader proofUploader;

        public IReadOnlyList<Academy> MyAcademies { get; private set; } = new List<Academy>();
        public IReadOnlyList<Academy> AllAcademies { get; private set; } = new List<Academy>();
        public IReadOnlyList<AcademyJoinRequest> Requests { get; private set; } = new List<AcademyJoinRequest>();
        public IReadOnlyList<AcademyGrade> Grades { get; private set; } = new List<AcademyGrade>();
        public IReadOnlyList<AcademyCourse> Courses { get; private set; } = new List<AcademyCourse>();
        public PaymentDetails Payment { get; private set; } = new PaymentDetails();
        public IReadOnlyList<CourseEnrollmentRequest> Enrollments { get; private set; } = new List<CourseEnrollmentRequest>();

        public string CurrentUserId => session?.CurrentUserId;

        public AcademyViewModel(IEducationService education = null, CurrentSessionHolder session = null,
            IImagePicker imagePicker = null, IPaymentProofUploader proofUploader = null)
        {
            this.education = education;
            this.session = session;
            this.imagePicker = imagePicker;
            this.proofUploader = proofUploader;
        }

        public async Task<Result> LoadMineAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListMyAcademiesAsync();
            IsBusy = false;
            if (result.IsSuccess) MyAcademies = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadAllAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListAcademiesAsync();
            IsBusy = false;
            if (result.IsSuccess) AllAcademies = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadRequestsAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListMyAcademyRequestsAsync();
            IsBusy = false;
            if (result.IsSuccess) Requests = result.Value;
            return Report(result);
        }

        public async Task<Result> CreateAsync(string name, string description, string city, string country)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.CreateAcademyAsync(name, description, city, country);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> UpdateAsync(Academy academy)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.UpdateAcademyAsync(academy);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeleteAsync(string academyId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.DeleteAcademyAsync(academyId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> RequestToJoinAsync(Academy academy, string studentName)
        {
            if (education == null) return PlaceholderOk();
            if (academy == null) return Fail("No academy selected.");
            IsBusy = true;
            var result = await education.RequestToJoinAsync(academy.Id, academy.Name, studentName);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> ApproveAsync(string requestId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ApproveRequestAsync(requestId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> RejectAsync(string requestId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.RejectRequestAsync(requestId);
            IsBusy = false;
            return Report(result);
        }

        // ── Grades & courses ──

        public async Task<Result> LoadGradesAsync(string academyId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListGradesAsync(academyId);
            IsBusy = false;
            if (result.IsSuccess) Grades = result.Value;
            return Report(result);
        }

        public async Task<Result> AddGradeAsync(string academyId, EducationStage stage, string title)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.AddGradeAsync(academyId, stage, title);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeleteGradeAsync(string gradeId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.DeleteGradeAsync(gradeId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> LoadCoursesAsync(string gradeId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListCoursesAsync(gradeId);
            IsBusy = false;
            if (result.IsSuccess) Courses = result.Value;
            return Report(result);
        }

        public async Task<Result> AddCourseAsync(string gradeId, string name, string description, double price, string timeFrame, bool freeTrial, string deadline)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.AddCourseAsync(gradeId, name, description, price, timeFrame, freeTrial, deadline);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> UpdateCourseAsync(AcademyCourse course)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.UpdateCourseAsync(course);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeleteCourseAsync(string courseId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.DeleteCourseAsync(courseId);
            IsBusy = false;
            return Report(result);
        }

        // ── Payment details ──

        public async Task<Result> LoadMyPaymentDetailsAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.GetMyPaymentDetailsAsync();
            IsBusy = false;
            if (result.IsSuccess) Payment = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadPaymentDetailsForAsync(string ownerId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.GetPaymentDetailsForAsync(ownerId);
            IsBusy = false;
            if (result.IsSuccess) Payment = result.Value;
            return Report(result);
        }

        public async Task<Result> SavePaymentDetailsAsync(string accountName, string fnb, string orange)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.SavePaymentDetailsAsync(accountName, fnb, orange);
            IsBusy = false;
            return Report(result);
        }

        // ── Paid enrollment ──

        public async Task<Result> RequestEnrollmentAsync(AcademyCourse course, Academy academy, string gradeTitle, string studentName, bool isFreeTrial)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.RequestEnrollmentAsync(course, academy, gradeTitle, studentName, isFreeTrial);
            IsBusy = false;
            return Report(result);
        }

        /// <summary>Lets the student pick a proof image and uploads it; returns the URL.</summary>
        public async Task<Result<string>> PickAndUploadProofAsync(string requestId)
        {
            if (imagePicker == null || proofUploader == null)
                return Result.Fail<string>("Image upload isn't available here.");

            var picked = await imagePicker.PickImageAsync();
            if (picked.Cancelled) return Result.Fail<string>("");      // silent cancel
            if (!picked.Success) { ErrorMessage = picked.Error; return Result.Fail<string>(picked.Error); }

            IsBusy = true;
            var upload = await proofUploader.UploadAsync(picked.Data, picked.Extension, requestId);
            IsBusy = false;
            if (upload.IsFailure) { ErrorMessage = upload.Error; }
            return upload;
        }

        public async Task<Result> SubmitPaymentAsync(string requestId, string reference, string proofUrl)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.SubmitEnrollmentPaymentAsync(requestId, reference, proofUrl);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> LoadOwnerEnrollmentsAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListEnrollmentRequestsForOwnerAsync();
            IsBusy = false;
            if (result.IsSuccess) Enrollments = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadMyEnrollmentsAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListMyEnrollmentRequestsAsync();
            IsBusy = false;
            if (result.IsSuccess) Enrollments = result.Value;
            return Report(result);
        }

        public async Task<Result> ConfirmEnrollmentAsync(CourseEnrollmentRequest request)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ConfirmEnrollmentAsync(request);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> RejectEnrollmentAsync(string requestId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.RejectEnrollmentAsync(requestId);
            IsBusy = false;
            return Report(result);
        }

        Result Report(Result result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        Result Report<T>(Result<T> result)
        {
            ErrorMessage = result.IsFailure ? result.Error : "";
            return result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
        }

        Result Fail(string message)
        {
            ErrorMessage = message;
            return Result.Fail(message);
        }
    }
}
