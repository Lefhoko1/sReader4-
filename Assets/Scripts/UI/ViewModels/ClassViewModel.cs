using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;
using SReader.Domains.Education.Models;
using SReader.Domains.Education.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>
    /// Backs the Classes module: classes under a grade, class enrolment, a
    /// tutor's roster, and the assignments assigned to a class. Forwards to
    /// IEducationService (classes/enrolment) and IAssignmentService
    /// (assignments). Null services = placeholder mode (no AppCompositionRoot).
    /// </summary>
    public sealed class ClassViewModel : ViewModelBase
    {
        readonly IEducationService education;
        readonly IAssignmentService assignments;

        public IReadOnlyList<AcademyClass> Classes { get; private set; } = new List<AcademyClass>();
        public IReadOnlyList<Enrollment> Roster { get; private set; } = new List<Enrollment>();
        public IReadOnlyList<Enrollment> MyClasses { get; private set; } = new List<Enrollment>();
        public IReadOnlyList<Assignment> Assignments { get; private set; } = new List<Assignment>();
        public IReadOnlyList<AcademyCourse> GradeSubjects { get; private set; } = new List<AcademyCourse>();

        public ClassViewModel(IEducationService education = null, IAssignmentService assignments = null)
        {
            this.education = education;
            this.assignments = assignments;
        }

        // ── Classes ──

        public async Task<Result> LoadClassesAsync(string gradeId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListClassesForGradeAsync(gradeId);
            IsBusy = false;
            if (result.IsSuccess) Classes = result.Value;
            return Report(result);
        }

        public async Task<Result> CreateClassAsync(Academy academy, AcademyGrade grade, string name, string description, int maxStudents)
        {
            if (education == null) return PlaceholderOk();
            if (academy == null || grade == null) return Fail("A grade and academy are required.");

            IsBusy = true;
            var result = await education.CreateClassAsync(new AcademyClass
            {
                AcademyId   = academy.Id,
                AcademyName = academy.Name,
                GradeId     = grade.Id,
                GradeTitle  = grade.Title,
                Name        = name,
                Description = description,
                MaxStudents = maxStudents
            });
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> UpdateClassAsync(AcademyClass academyClass, string name, string description, int maxStudents)
        {
            if (education == null) return PlaceholderOk();
            if (academyClass == null) return Fail("A class is required.");

            academyClass.Name        = name;
            academyClass.Description = description;
            academyClass.MaxStudents = maxStudents;

            IsBusy = true;
            var result = await education.UpdateClassAsync(academyClass);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeleteClassAsync(string classId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.DeleteClassAsync(classId);
            IsBusy = false;
            return Report(result);
        }

        // ── Enrolment ──

        public async Task<Result> EnrollAsync(AcademyClass academyClass, string studentName)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.EnrollInClassAsync(academyClass, studentName);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> LeaveAsync(string classId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.LeaveClassAsync(classId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> LoadMyClassesAsync()
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListMyClassesAsync();
            IsBusy = false;
            if (result.IsSuccess) MyClasses = result.Value;
            return Report(result);
        }

        public async Task<Result> LoadRosterAsync(string classId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListClassRosterAsync(classId);
            IsBusy = false;
            if (result.IsSuccess) Roster = result.Value;
            return Report(result);
        }

        public bool IsEnrolledIn(string classId)
            => MyClasses.Any(e => e.ClassId == classId);

        // ── Subjects taught in a class (tutor links grade subjects to a class) ──

        public async Task<Result> LoadGradeSubjectsAsync(string gradeId)
        {
            if (education == null) return PlaceholderOk();
            IsBusy = true;
            var result = await education.ListCoursesAsync(gradeId);
            IsBusy = false;
            if (result.IsSuccess) GradeSubjects = result.Value;
            return Report(result);
        }

        public async Task<Result> AssignSubjectAsync(AcademyCourse subject, AcademyClass academyClass)
        {
            if (education == null) return PlaceholderOk();
            if (subject == null || academyClass == null) return Fail("A subject and class are required.");

            IsBusy = true;
            var result = await education.SetCourseClassAsync(subject.Id, academyClass.Id, academyClass.Name);
            IsBusy = false;
            if (result.IsSuccess) { subject.ClassId = academyClass.Id; subject.ClassName = academyClass.Name; }
            return Report(result);
        }

        public async Task<Result> UnassignSubjectAsync(AcademyCourse subject)
        {
            if (education == null) return PlaceholderOk();
            if (subject == null) return Fail("A subject is required.");

            IsBusy = true;
            var result = await education.SetCourseClassAsync(subject.Id, null, null);
            IsBusy = false;
            if (result.IsSuccess) { subject.ClassId = null; subject.ClassName = null; }
            return Report(result);
        }

        // ── Assignments ──

        public async Task<Result> LoadAssignmentsAsync(string classId)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var result = await assignments.ListForClassAsync(classId);
            IsBusy = false;
            if (result.IsSuccess) Assignments = result.Value;
            return Report(result);
        }

        public async Task<Result> CreateAssignmentAsync(string classId, string title, string instructions, DateTime dueDate, int maxScore)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var result = await assignments.CreateForClassAsync(classId, title, instructions, dueDate, maxScore);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> UpdateAssignmentAsync(Assignment assignment, string title, string instructions, DateTime dueDate, int maxScore)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var result = await assignments.UpdateAsync(assignment, title, instructions, dueDate, maxScore);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> DeleteAssignmentAsync(string assignmentId)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var result = await assignments.DeleteAsync(assignmentId);
            IsBusy = false;
            return Report(result);
        }

        public async Task<Result> SaveAssignmentContentAsync(string assignmentId, AssignmentContent content)
        {
            if (assignments == null) return PlaceholderOk();
            IsBusy = true;
            var result = await assignments.SetContentAsync(assignmentId, AssignmentContentCodec.ToJson(content));
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
