using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Education.Models;
using SReader.Domains.Education.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// PostgREST-backed education repository over {SupabaseUrl}/rest/v1.
    /// Academies and academy_join_requests are fully implemented; classes /
    /// modules / lessons / enrollments remain Phase-3 stubs. Calls carry the
    /// current user's access token so Supabase Row-Level Security applies
    /// (a tutor only sees/edits their own academies and their requests).
    /// </summary>
    public sealed class SupabaseEducationRepository : SupabaseRepositoryBase, IEducationRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseEducationRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        // ── Academies ──────────────────────────────────────────────────────

        public Task<Result<IReadOnlyList<Academy>>> ListAcademiesAsync(CancellationToken ct = default)
            => QueryAcademies($"{RestUrl}/academies?select=*&order=created_at.desc", ct);

        public Task<Result<IReadOnlyList<Academy>>> ListAcademiesByOwnerAsync(string ownerId, CancellationToken ct = default)
            => QueryAcademies($"{RestUrl}/academies?owner_id=eq.{Uri.EscapeDataString(ownerId)}&select=*&order=created_at.desc", ct);

        async Task<Result<IReadOnlyList<Academy>>> QueryAcademies(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<Academy>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<Academy>>(response.Error);

            var rows = ParseArray<AcademyDto>(response.Value);
            var list = new List<Academy>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<Academy>>(list);
        }

        public async Task<Result<Academy>> GetAcademyAsync(string academyId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<Academy>(configured.Error);

            var url = $"{RestUrl}/academies?id=eq.{Uri.EscapeDataString(academyId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<Academy>(response.Error);

            var rows = ParseArray<AcademyDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<Academy>("Academy not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<Academy>> CreateAcademyAsync(Academy academy, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<Academy>(configured.Error);

            var body = JsonUtility.ToJson(AcademyWriteDto.From(academy));
            // return=representation so we get the generated id/created_at back.
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/academies",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<Academy>(response.Error);

            var rows = ParseArray<AcademyDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<Academy>("Academy was created but not returned.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result> UpdateAcademyAsync(Academy academy, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(AcademyWriteDto.From(academy));
            var url = $"{RestUrl}/academies?id=eq.{Uri.EscapeDataString(academy.Id)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteAcademyAsync(string academyId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/academies?id=eq.{Uri.EscapeDataString(academyId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Join requests ───────────────────────────────────────────────────

        public async Task<Result> CreateJoinRequestAsync(AcademyJoinRequest request, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(JoinRequestWriteDto.From(request));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/academy_join_requests",
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<IReadOnlyList<AcademyJoinRequest>>> ListJoinRequestsForOwnerAsync(CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AcademyJoinRequest>>(configured.Error);

            // RLS limits the rows to requests for academies this tutor owns.
            var url = $"{RestUrl}/academy_join_requests?select=*&order=created_at.desc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AcademyJoinRequest>>(response.Error);

            var rows = ParseArray<JoinRequestDto>(response.Value);
            var list = new List<AcademyJoinRequest>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AcademyJoinRequest>>(list);
        }

        public async Task<Result> UpdateJoinRequestStatusAsync(string requestId, JoinRequestStatus status, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new StatusPatch { status = status.ToString().ToLowerInvariant() });
            var url = $"{RestUrl}/academy_join_requests?id=eq.{Uri.EscapeDataString(requestId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Grades ──────────────────────────────────────────────────────────

        public async Task<Result<IReadOnlyList<AcademyGrade>>> ListGradesAsync(string academyId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AcademyGrade>>(configured.Error);

            var url = $"{RestUrl}/academy_grades?academy_id=eq.{Uri.EscapeDataString(academyId)}&select=*&order=created_at.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AcademyGrade>>(response.Error);

            var rows = ParseArray<GradeDto>(response.Value);
            var list = new List<AcademyGrade>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AcademyGrade>>(list);
        }

        public async Task<Result<AcademyGrade>> CreateGradeAsync(AcademyGrade grade, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AcademyGrade>(configured.Error);

            var body = JsonUtility.ToJson(GradeWriteDto.From(grade));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/academy_grades",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<AcademyGrade>(response.Error);

            var rows = ParseArray<GradeDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AcademyGrade>("Grade was created but not returned.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result> DeleteGradeAsync(string gradeId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/academy_grades?id=eq.{Uri.EscapeDataString(gradeId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Courses ─────────────────────────────────────────────────────────

        public async Task<Result<IReadOnlyList<AcademyCourse>>> ListCoursesAsync(string gradeId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AcademyCourse>>(configured.Error);

            var url = $"{RestUrl}/academy_courses?grade_id=eq.{Uri.EscapeDataString(gradeId)}&select=*&order=created_at.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AcademyCourse>>(response.Error);

            var rows = ParseArray<CourseDto>(response.Value);
            var list = new List<AcademyCourse>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AcademyCourse>>(list);
        }

        public async Task<Result<AcademyCourse>> GetCourseAsync(string courseId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AcademyCourse>(configured.Error);

            var url = $"{RestUrl}/academy_courses?id=eq.{Uri.EscapeDataString(courseId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AcademyCourse>(response.Error);

            var rows = ParseArray<CourseDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AcademyCourse>("Course not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<AcademyCourse>> CreateCourseAsync(AcademyCourse course, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AcademyCourse>(configured.Error);

            var body = JsonUtility.ToJson(CourseWriteDto.From(course));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/academy_courses",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<AcademyCourse>(response.Error);

            var rows = ParseArray<CourseDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AcademyCourse>("Course was created but not returned.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result> SetCourseClassAsync(string courseId, string classId, string className, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            // Hand-build the JSON so an empty class_id is sent as real JSON null
            // (JsonUtility would emit "", which the uuid column rejects).
            var classIdJson = string.IsNullOrWhiteSpace(classId) ? "null" : "\"" + classId + "\"";
            var classNameJson = JsonStringOrNull(className);
            var body = $"{{\"class_id\":{classIdJson},\"class_name\":{classNameJson}}}";

            var url = $"{RestUrl}/academy_courses?id=eq.{Uri.EscapeDataString(courseId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // A JSON string literal, or the literal null. Escapes the characters JSON requires.
        static string JsonStringOrNull(string s)
        {
            if (s == null) return "null";
            var sb = new System.Text.StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:   sb.Append(ch);     break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        public async Task<Result> UpdateCourseAsync(AcademyCourse course, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(CourseWriteDto.From(course));
            var url = $"{RestUrl}/academy_courses?id=eq.{Uri.EscapeDataString(course.Id)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteCourseAsync(string courseId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/academy_courses?id=eq.{Uri.EscapeDataString(courseId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Payment details ─────────────────────────────────────────────────

        public async Task<Result<PaymentDetails>> GetPaymentDetailsAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<PaymentDetails>(configured.Error);

            var url = $"{RestUrl}/tutor_payment_details?user_id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<PaymentDetails>(response.Error);

            var rows = ParseArray<PaymentDetailsDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? new PaymentDetails { UserId = userId } : rows[0].ToModel());
        }

        public async Task<Result> UpsertPaymentDetailsAsync(PaymentDetails details, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(PaymentDetailsDto.From(details));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/tutor_payment_details",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Enrollment requests ─────────────────────────────────────────────

        public async Task<Result> CreateEnrollmentRequestAsync(CourseEnrollmentRequest request, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(EnrollmentWriteDto.From(request));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/course_enrollment_requests",
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> SubmitEnrollmentPaymentAsync(string requestId, string paymentReference, string paymentProofUrl, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new PaymentSubmitPatch
            {
                status = EnrollmentStatus.PaymentSubmitted.ToString().ToLowerInvariant(),
                payment_reference = paymentReference,
                payment_proof_url = paymentProofUrl
            });
            var url = $"{RestUrl}/course_enrollment_requests?id=eq.{Uri.EscapeDataString(requestId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UpdateEnrollmentStatusAsync(string requestId, EnrollmentStatus status, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new EnrollStatusPatch { status = status.ToString().ToLowerInvariant() });
            var url = $"{RestUrl}/course_enrollment_requests?id=eq.{Uri.EscapeDataString(requestId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListEnrollmentRequestsForOwnerAsync(CancellationToken ct = default)
            => QueryEnrollments($"{RestUrl}/course_enrollment_requests?owner_id=eq.{Uri.EscapeDataString(session?.CurrentUserId ?? "")}&select=*&order=created_at.desc", ct);

        public Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> ListMyEnrollmentRequestsAsync(CancellationToken ct = default)
            => QueryEnrollments($"{RestUrl}/course_enrollment_requests?student_id=eq.{Uri.EscapeDataString(session?.CurrentUserId ?? "")}&select=*&order=created_at.desc", ct);

        async Task<Result<IReadOnlyList<CourseEnrollmentRequest>>> QueryEnrollments(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<CourseEnrollmentRequest>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<CourseEnrollmentRequest>>(response.Error);

            var rows = ParseArray<EnrollmentDto>(response.Value);
            var list = new List<CourseEnrollmentRequest>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<CourseEnrollmentRequest>>(list);
        }

        // ── Classes (under a grade, within an academy) ───────────────────────

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesAsync(string academyId, CancellationToken ct = default)
            => QueryClasses($"{RestUrl}/academy_classes?academy_id=eq.{Uri.EscapeDataString(academyId)}&select=*&order=created_at.desc", ct);

        public Task<Result<IReadOnlyList<AcademyClass>>> ListClassesByGradeAsync(string gradeId, CancellationToken ct = default)
            => QueryClasses($"{RestUrl}/academy_classes?grade_id=eq.{Uri.EscapeDataString(gradeId)}&select=*&order=created_at.desc", ct);

        async Task<Result<IReadOnlyList<AcademyClass>>> QueryClasses(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<AcademyClass>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<AcademyClass>>(response.Error);

            var rows = ParseArray<ClassDto>(response.Value);
            var list = new List<AcademyClass>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<AcademyClass>>(list);
        }

        public async Task<Result<AcademyClass>> GetClassAsync(string classId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AcademyClass>(configured.Error);

            var url = $"{RestUrl}/academy_classes?id=eq.{Uri.EscapeDataString(classId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<AcademyClass>(response.Error);

            var rows = ParseArray<ClassDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AcademyClass>("Class not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<AcademyClass>> CreateClassAsync(AcademyClass academyClass, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AcademyClass>(configured.Error);

            var body = JsonUtility.ToJson(ClassWriteDto.From(academyClass));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/academy_classes",
                Settings.SupabaseAnonKey, body, Token, ct, "return=representation");
            if (response.IsFailure) return Result.Fail<AcademyClass>(response.Error);

            var rows = ParseArray<ClassDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<AcademyClass>("Class was created but not returned.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result> UpdateClassAsync(AcademyClass academyClass, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(ClassWriteDto.From(academyClass));
            var url = $"{RestUrl}/academy_classes?id=eq.{Uri.EscapeDataString(academyClass.Id)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteClassAsync(string classId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/academy_classes?id=eq.{Uri.EscapeDataString(classId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public Task<Result<IReadOnlyList<Module>>> ListModulesAsync(string classId, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<Module>>("List modules");

        public Task<Result<IReadOnlyList<Lesson>>> ListLessonsAsync(string moduleId, CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<Lesson>>("List lessons");

        // ── Class enrolments ─────────────────────────────────────────────────

        public async Task<Result> EnrollAsync(Enrollment enrollment, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(ClassEnrollmentWriteDto.From(enrollment));
            // on_conflict on the (class_id, student_id) unique key + merge-duplicates
            // makes re-enrolling the same student idempotent instead of a 409.
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post,
                $"{RestUrl}/class_enrollments?on_conflict=class_id,student_id",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UnenrollAsync(string studentId, string classId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/class_enrollments?student_id=eq.{Uri.EscapeDataString(studentId)}&class_id=eq.{Uri.EscapeDataString(classId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public Task<Result<IReadOnlyList<Enrollment>>> ListEnrollmentsAsync(string studentId, CancellationToken ct = default)
            => QueryClassEnrollments($"{RestUrl}/class_enrollments?student_id=eq.{Uri.EscapeDataString(studentId)}&select=*&order=created_at.desc", ct);

        public Task<Result<IReadOnlyList<Enrollment>>> ListClassRosterAsync(string classId, CancellationToken ct = default)
            => QueryClassEnrollments($"{RestUrl}/class_enrollments?class_id=eq.{Uri.EscapeDataString(classId)}&select=*&order=created_at.asc", ct);

        async Task<Result<IReadOnlyList<Enrollment>>> QueryClassEnrollments(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<Enrollment>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<Enrollment>>(response.Error);

            var rows = ParseArray<ClassEnrollmentDto>(response.Value);
            var list = new List<Enrollment>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<Enrollment>>(list);
        }

        public async Task<Result<int>> CountEnrollmentsAsync(string classId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<int>(configured.Error);

            var url = $"{RestUrl}/class_enrollments?class_id=eq.{Uri.EscapeDataString(classId)}&select=id";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<int>(response.Error);

            return Result.Ok(ParseArray<ClassEnrollmentDto>(response.Value).Length);
        }

        // ── DTOs (JsonUtility needs public fields with snake_case names) ──

        [Serializable]
        class AcademyDto
        {
            public string id;
            public string owner_id;
            public string name;
            public string description;
            public string city;
            public string country;

            public Academy ToModel() => new Academy
            {
                Id = id,
                OwnerId = owner_id,
                Name = name,
                Description = description,
                City = city,
                Country = country
            };
        }

        // Insert/update payload — no id field, so PostgREST generates the uuid
        // (a serialized "id":"" would fail as invalid uuid).
        [Serializable]
        class AcademyWriteDto
        {
            public string owner_id;
            public string name;
            public string description;
            public string city;
            public string country;

            public static AcademyWriteDto From(Academy a) => new AcademyWriteDto
            {
                owner_id = a.OwnerId,
                name = a.Name,
                description = a.Description,
                city = a.City,
                country = a.Country
            };
        }

        [Serializable]
        class JoinRequestDto
        {
            public string id;
            public string academy_id;
            public string academy_name;
            public string student_id;
            public string student_name;
            public string status;

            public AcademyJoinRequest ToModel() => new AcademyJoinRequest
            {
                Id = id,
                AcademyId = academy_id,
                AcademyName = academy_name,
                StudentId = student_id,
                StudentName = student_name,
                Status = ParseEnum(status, JoinRequestStatus.Pending)
            };
        }

        // Insert payload — no id field (PostgREST generates the uuid).
        [Serializable]
        class JoinRequestWriteDto
        {
            public string academy_id;
            public string academy_name;
            public string student_id;
            public string student_name;
            public string status;

            public static JoinRequestWriteDto From(AcademyJoinRequest r) => new JoinRequestWriteDto
            {
                academy_id = r.AcademyId,
                academy_name = r.AcademyName,
                student_id = r.StudentId,
                student_name = r.StudentName,
                status = r.Status.ToString().ToLowerInvariant()
            };
        }

        [Serializable]
        class GradeDto
        {
            public string id;
            public string academy_id;
            public string stage;
            public string title;

            public AcademyGrade ToModel() => new AcademyGrade
            {
                Id = id,
                AcademyId = academy_id,
                Stage = ParseEnum(stage, EducationStage.Primary),
                Title = title
            };
        }

        [Serializable]
        class GradeWriteDto
        {
            public string academy_id;
            public string stage;
            public string title;

            public static GradeWriteDto From(AcademyGrade g) => new GradeWriteDto
            {
                academy_id = g.AcademyId,
                stage = g.Stage.ToString().ToLowerInvariant(),
                title = g.Title
            };
        }

        [Serializable]
        class CourseDto
        {
            public string id;
            public string grade_id;
            public string name;
            public string description;
            public double price;
            public string time_frame;
            public bool free_trial;
            public string deadline;
            public string class_id;
            public string class_name;

            public AcademyCourse ToModel() => new AcademyCourse
            {
                Id = id,
                GradeId = grade_id,
                Name = name,
                Description = description,
                Price = price,
                TimeFrame = time_frame,
                FreeTrial = free_trial,
                Deadline = deadline,
                ClassId = class_id,
                ClassName = class_name
            };
        }

        [Serializable]
        class CourseWriteDto
        {
            public string grade_id;
            public string name;
            public string description;
            public double price;
            public string time_frame;
            public bool free_trial;
            public string deadline;

            // class_id/class_name are deliberately NOT written here: JsonUtility
            // serialises a null string as "", which Postgres rejects for the uuid
            // class_id column. The subject↔class link is set separately via
            // SetCourseClassAsync (a hand-built PATCH that can send real JSON null).
            public static CourseWriteDto From(AcademyCourse c) => new CourseWriteDto
            {
                grade_id = c.GradeId,
                name = c.Name,
                description = c.Description,
                price = c.Price,
                time_frame = c.TimeFrame,
                free_trial = c.FreeTrial,
                deadline = string.IsNullOrWhiteSpace(c.Deadline) ? null : c.Deadline
            };
        }

        [Serializable]
        class PaymentDetailsDto
        {
            public string user_id;
            public string account_name;
            public string fnb_account;
            public string orange_money;

            public PaymentDetails ToModel() => new PaymentDetails
            {
                UserId = user_id,
                AccountName = account_name,
                FnbAccount = fnb_account,
                OrangeMoney = orange_money
            };

            public static PaymentDetailsDto From(PaymentDetails p) => new PaymentDetailsDto
            {
                user_id = p.UserId,
                account_name = p.AccountName,
                fnb_account = p.FnbAccount,
                orange_money = p.OrangeMoney
            };
        }

        [Serializable]
        class EnrollmentDto
        {
            public string id;
            public string course_id;
            public string course_name;
            public string grade_title;
            public string academy_id;
            public string academy_name;
            public string owner_id;
            public string student_id;
            public string student_name;
            public string status;
            public bool is_free_trial;
            public string payment_reference;
            public string payment_proof_url;

            public CourseEnrollmentRequest ToModel() => new CourseEnrollmentRequest
            {
                Id = id,
                CourseId = course_id,
                CourseName = course_name,
                GradeTitle = grade_title,
                AcademyId = academy_id,
                AcademyName = academy_name,
                OwnerId = owner_id,
                StudentId = student_id,
                StudentName = student_name,
                Status = ParseEnum(status, EnrollmentStatus.Requested),
                IsFreeTrial = is_free_trial,
                PaymentReference = payment_reference,
                PaymentProofUrl = payment_proof_url
            };
        }

        [Serializable]
        class EnrollmentWriteDto
        {
            public string course_id;
            public string course_name;
            public string grade_title;
            public string academy_id;
            public string academy_name;
            public string owner_id;
            public string student_id;
            public string student_name;
            public string status;
            public bool is_free_trial;

            public static EnrollmentWriteDto From(CourseEnrollmentRequest r) => new EnrollmentWriteDto
            {
                course_id = r.CourseId,
                course_name = r.CourseName,
                grade_title = r.GradeTitle,
                academy_id = r.AcademyId,
                academy_name = r.AcademyName,
                owner_id = r.OwnerId,
                student_id = r.StudentId,
                student_name = r.StudentName,
                status = r.Status.ToString().ToLowerInvariant(),
                is_free_trial = r.IsFreeTrial
            };
        }

        [Serializable]
        class ClassDto
        {
            public string id;
            public string academy_id;
            public string grade_id;
            public string owner_id;
            public string academy_name;
            public string grade_title;
            public string name;
            public string description;
            public int max_students;
            public string created_at;

            public AcademyClass ToModel() => new AcademyClass
            {
                Id          = id,
                AcademyId   = academy_id,
                GradeId     = grade_id,
                OwnerId     = owner_id,
                AcademyName = academy_name,
                GradeTitle  = grade_title,
                Name        = name,
                Description = description,
                MaxStudents = max_students,
                CreatedAt   = ParseDate(created_at)
            };
        }

        [Serializable]
        class ClassWriteDto
        {
            public string academy_id;
            public string grade_id;
            public string owner_id;
            public string academy_name;
            public string grade_title;
            public string name;
            public string description;
            public int max_students;

            public static ClassWriteDto From(AcademyClass c) => new ClassWriteDto
            {
                academy_id   = c.AcademyId,
                grade_id     = c.GradeId,
                owner_id     = c.OwnerId,
                academy_name = c.AcademyName,
                grade_title  = c.GradeTitle,
                name         = c.Name,
                description  = c.Description,
                max_students = c.MaxStudents
            };
        }

        [Serializable]
        class ClassEnrollmentDto
        {
            public string id;
            public string class_id;
            public string student_id;
            public string student_name;
            public string class_name;
            public string grade_title;
            public string academy_id;
            public string academy_name;
            public string created_at;

            public Enrollment ToModel() => new Enrollment
            {
                Id             = id,
                ClassId        = class_id,
                StudentId      = student_id,
                StudentName    = student_name,
                ClassName      = class_name,
                GradeTitle     = grade_title,
                AcademyId      = academy_id,
                AcademyName    = academy_name,
                EnrollmentDate = ParseDate(created_at)
            };
        }

        [Serializable]
        class ClassEnrollmentWriteDto
        {
            public string class_id;
            public string student_id;
            public string student_name;
            public string class_name;
            public string grade_title;
            public string academy_id;
            public string academy_name;

            public static ClassEnrollmentWriteDto From(Enrollment e) => new ClassEnrollmentWriteDto
            {
                class_id     = e.ClassId,
                student_id   = e.StudentId,
                student_name = e.StudentName,
                class_name   = e.ClassName,
                grade_title  = e.GradeTitle,
                academy_id   = e.AcademyId,
                academy_name = e.AcademyName
            };
        }

        [Serializable] class PaymentSubmitPatch { public string status; public string payment_reference; public string payment_proof_url; }
        [Serializable] class EnrollStatusPatch { public string status; }

        [Serializable] class StatusPatch { public string status; }

        static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct
            => Enum.TryParse<TEnum>(value, true, out var parsed) ? parsed : fallback;

        static DateTime ParseDate(string value)
            => DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var d)
                ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : DateTime.UtcNow;

        // JsonUtility cannot parse a top-level JSON array, so wrap it first.
        [Serializable] class Wrapper<T> { public T[] items; }

        static T[] ParseArray<T>(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "[]") return Array.Empty<T>();
            try
            {
                var wrapped = JsonUtility.FromJson<Wrapper<T>>("{\"items\":" + json + "}");
                return wrapped?.items ?? Array.Empty<T>();
            }
            catch
            {
                return Array.Empty<T>();
            }
        }
    }
}
