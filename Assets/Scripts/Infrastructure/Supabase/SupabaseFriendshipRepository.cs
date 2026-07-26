using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Identity.Models;
using SReader.Domains.Social.Models;
using SReader.Domains.Social.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// PostgREST-backed friendship repository over {SupabaseUrl}/rest/v1.
    /// Tables: friendships, friend_settings. Discovery reads the friend-safe
    /// <c>directory_profiles</c> view (users joined to their latest avatar, no
    /// private columns). Calls carry the current user's access token so RLS
    /// applies — a user only ever sees the friendships they're part of.
    /// </summary>
    public sealed class SupabaseFriendshipRepository : SupabaseRepositoryBase, IFriendshipRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseFriendshipRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        // ── Friendships ─────────────────────────────────────────────────────

        public async Task<Result<Friendship>> GetAsync(string friendshipId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<Friendship>(configured.Error);

            var url = $"{RestUrl}/friendships?id=eq.{Uri.EscapeDataString(friendshipId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<Friendship>(response.Error);

            var rows = ParseArray<FriendshipDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<Friendship>("Friendship not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<Friendship>> FindBetweenAsync(string userA, string userB, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<Friendship>(configured.Error);

            var a = Uri.EscapeDataString(userA);
            var b = Uri.EscapeDataString(userB);
            // either direction: (requester=A & recipient=B) OR (requester=B & recipient=A)
            var or = $"or=(and(requester_id.eq.{a},recipient_id.eq.{b}),and(requester_id.eq.{b},recipient_id.eq.{a}))";
            var url = $"{RestUrl}/friendships?{or}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<Friendship>(response.Error);

            var rows = ParseArray<FriendshipDto>(response.Value);
            // Not finding a row is a valid answer (no relationship yet), not an error.
            return Result.Ok(rows.Length == 0 ? null : rows[0].ToModel());
        }

        public Task<Result<IReadOnlyList<Friendship>>> ListForUserAsync(string userId, FriendshipStatus status, CancellationToken ct = default)
        {
            var id = Uri.EscapeDataString(userId);
            var s = Uri.EscapeDataString(status.ToString().ToLowerInvariant());
            var url = $"{RestUrl}/friendships?or=(requester_id.eq.{id},recipient_id.eq.{id})&status=eq.{s}&select=*&order=created_at.desc";
            return QueryFriendships(url, ct);
        }

        public Task<Result<IReadOnlyList<Friendship>>> ListAllForUserAsync(string userId, CancellationToken ct = default)
        {
            var id = Uri.EscapeDataString(userId);
            var url = $"{RestUrl}/friendships?or=(requester_id.eq.{id},recipient_id.eq.{id})&select=*&order=created_at.desc";
            return QueryFriendships(url, ct);
        }

        async Task<Result<IReadOnlyList<Friendship>>> QueryFriendships(string url, CancellationToken ct)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<Friendship>>(configured.Error);

            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<Friendship>>(response.Error);

            var rows = ParseArray<FriendshipDto>(response.Value);
            var list = new List<Friendship>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<Friendship>>(list);
        }

        public async Task<Result> CreateAsync(Friendship friendship, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(FriendshipWriteDto.From(friendship));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/friendships",
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> UpdateStatusAsync(string friendshipId, FriendshipStatus status, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new StatusPatch { status = status.ToString().ToLowerInvariant() });
            var url = $"{RestUrl}/friendships?id=eq.{Uri.EscapeDataString(friendshipId)}";
            var response = await SupabaseHttp.SendAsync(new HttpMethod("PATCH"), url,
                Settings.SupabaseAnonKey, body, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> DeleteAsync(string friendshipId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var url = $"{RestUrl}/friendships?id=eq.{Uri.EscapeDataString(friendshipId)}";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Delete, url,
                Settings.SupabaseAnonKey, null, Token, ct, "return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── Friend settings ──────────────────────────────────────────────────

        public async Task<Result<FriendSettings>> GetFriendSettingsAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<FriendSettings>(configured.Error);

            var url = $"{RestUrl}/friend_settings?user_id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<FriendSettings>(response.Error);

            var rows = ParseArray<FriendSettingsDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? new FriendSettings { UserId = userId } : rows[0].ToModel());
        }

        public async Task<Result> UpsertFriendSettingsAsync(FriendSettings settings, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(FriendSettingsDto.From(settings));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/friend_settings",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── People directory ─────────────────────────────────────────────────

        public async Task<Result<IReadOnlyList<PersonSummary>>> ListPeopleAsync(CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<PersonSummary>>(configured.Error);

            var url = $"{RestUrl}/directory_profiles?select=*&order=display_name.asc";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<PersonSummary>>(response.Error);

            var rows = ParseArray<DirectoryDto>(response.Value);
            var list = new List<PersonSummary>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<PersonSummary>>(list);
        }

        public async Task<Result<PersonSummary>> GetPersonAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<PersonSummary>(configured.Error);

            var url = $"{RestUrl}/directory_profiles?id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<PersonSummary>(response.Error);

            var rows = ParseArray<DirectoryDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<PersonSummary>("Person not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<IReadOnlyList<StudentSubject>>> ListStudentAcademicsAsync(string studentId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<IReadOnlyList<StudentSubject>>(configured.Error);

            var url = $"{RestUrl}/student_academics?student_id=eq.{Uri.EscapeDataString(studentId)}&select=*";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<IReadOnlyList<StudentSubject>>(response.Error);

            var rows = ParseArray<StudentAcademicDto>(response.Value);
            var list = new List<StudentSubject>(rows.Length);
            foreach (var r in rows) list.Add(r.ToModel());
            return Result.Ok<IReadOnlyList<StudentSubject>>(list);
        }

        // ── PostgREST DTOs ────────────────────────────────────────────────────

        [Serializable]
        class FriendshipDto
        {
            public string id;
            public string requester_id;
            public string recipient_id;
            public string status;
            public string created_at;

            public Friendship ToModel() => new Friendship
            {
                Id = id,
                RequesterId = requester_id,
                RecipientId = recipient_id,
                Status = ParseEnum(status, FriendshipStatus.Pending),
                CreatedAt = ParseDate(created_at)
            };
        }

        [Serializable]
        class FriendshipWriteDto
        {
            public string requester_id;
            public string recipient_id;
            public string status;

            public static FriendshipWriteDto From(Friendship f) => new FriendshipWriteDto
            {
                requester_id = f.RequesterId,
                recipient_id = f.RecipientId,
                status = f.Status.ToString().ToLowerInvariant()
            };
        }

        [Serializable]
        class DirectoryDto
        {
            public string id;
            public string username;
            public string display_name;
            public string role;
            public string avatar_url;
            public string bio;
            public string country;

            public PersonSummary ToModel() => new PersonSummary
            {
                UserId = id,
                Username = username,
                DisplayName = display_name,
                Role = ParseEnum(role, UserRole.Student),
                AvatarUrl = avatar_url,
                Bio = bio,
                Country = country
            };
        }

        [Serializable]
        class StudentAcademicDto
        {
            public string student_id;
            public string academy_id;
            public string academy_name;
            public string grade_title;
            public string course_name;
            public string tutor_id;
            public string tutor_name;

            public StudentSubject ToModel() => new StudentSubject
            {
                AcademyId = academy_id,
                AcademyName = academy_name,
                GradeTitle = grade_title,
                CourseName = course_name,
                TutorId = tutor_id,
                TutorName = tutor_name
            };
        }

        [Serializable]
        class FriendSettingsDto
        {
            public string user_id;
            public bool can_see_profile;
            public bool can_see_schedule;
            public bool can_see_assignments;
            public bool can_see_location;
            public bool can_see_online_status;

            public FriendSettings ToModel() => new FriendSettings
            {
                UserId = user_id,
                CanSeeProfile = can_see_profile,
                CanSeeSchedule = can_see_schedule,
                CanSeeAssignments = can_see_assignments,
                CanSeeLocation = can_see_location,
                CanSeeOnlineStatus = can_see_online_status
            };

            public static FriendSettingsDto From(FriendSettings s) => new FriendSettingsDto
            {
                user_id = s.UserId,
                can_see_profile = s.CanSeeProfile,
                can_see_schedule = s.CanSeeSchedule,
                can_see_assignments = s.CanSeeAssignments,
                can_see_location = s.CanSeeLocation,
                can_see_online_status = s.CanSeeOnlineStatus
            };
        }

        [Serializable] class StatusPatch { public string status; }

        static DateTime ParseDate(string value)
            => DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var d)
                ? DateTime.SpecifyKind(d, DateTimeKind.Utc) : DateTime.UtcNow;

        static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct
            => Enum.TryParse<TEnum>(value, true, out var parsed) ? parsed : fallback;

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
