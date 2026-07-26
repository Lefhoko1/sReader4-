using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// PostgREST-backed identity repository over {SupabaseUrl}/rest/v1.
    /// Tables: users, user_profiles, privacy_settings — each keyed by the
    /// auth user's UUID. Calls carry the current user's access token so
    /// Supabase Row-Level Security applies.
    /// </summary>
    public sealed class SupabaseUserRepository : SupabaseRepositoryBase, IUserRepository
    {
        readonly CurrentSessionHolder session;

        string RestUrl => Settings.SupabaseUrl.TrimEnd('/') + "/rest/v1";
        string Token => session?.Session?.AccessToken;

        public SupabaseUserRepository(AppSettings settings, CurrentSessionHolder session) : base(settings)
        {
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<User>> GetByIdAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<User>(configured.Error);

            var url = $"{RestUrl}/users?id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<User>(response.Error);

            var rows = ParseArray<UserDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<User>("User not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<User>(configured.Error);

            var url = $"{RestUrl}/users?email=eq.{Uri.EscapeDataString(email)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<User>(response.Error);

            var rows = ParseArray<UserDto>(response.Value);
            if (rows.Length == 0) return Result.Fail<User>("User not found.");
            return Result.Ok(rows[0].ToModel());
        }

        public async Task<Result> UpdateAsync(User user, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(UserDto.From(user));
            // Upsert keyed by id — create the row on first save, update thereafter.
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/users",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<UserProfile>(configured.Error);

            var url = $"{RestUrl}/user_profiles?user_id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<UserProfile>(response.Error);

            var rows = ParseArray<ProfileDto>(response.Value);
            // A user without a saved profile yet is not an error — return an empty one.
            return Result.Ok(rows.Length == 0 ? new UserProfile { UserId = userId } : rows[0].ToModel());
        }

        public async Task<Result> UpsertProfileAsync(UserProfile profile, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(ProfileDto.From(profile));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/user_profiles",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<PrivacySettings>> GetPrivacySettingsAsync(string userId, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<PrivacySettings>(configured.Error);

            var url = $"{RestUrl}/privacy_settings?user_id=eq.{Uri.EscapeDataString(userId)}&select=*&limit=1";
            var response = await SupabaseHttp.SendAsync(HttpMethod.Get, url, Settings.SupabaseAnonKey, bearerToken: Token, ct: ct);
            if (response.IsFailure) return Result.Fail<PrivacySettings>(response.Error);

            var rows = ParseArray<PrivacyDto>(response.Value);
            return Result.Ok(rows.Length == 0 ? new PrivacySettings { UserId = userId } : rows[0].ToModel());
        }

        public async Task<Result> UpsertPrivacySettingsAsync(PrivacySettings settings, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(PrivacyDto.From(settings));
            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{RestUrl}/privacy_settings",
                Settings.SupabaseAnonKey, body, Token, ct, "resolution=merge-duplicates,return=minimal");
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        // ── PostgREST DTOs (JsonUtility needs public fields with snake_case names) ──

        [Serializable]
        class UserDto
        {
            public string id;
            public string email;
            public string username;
            public string display_name;
            public string role;
            public string status;

            public User ToModel() => new User
            {
                Id = id,
                Email = email,
                Username = username,
                DisplayName = display_name,
                Role = ParseEnum(role, UserRole.Student),
                Status = ParseEnum(status, UserStatus.Active)
            };

            public static UserDto From(User u) => new UserDto
            {
                id = u.Id,
                email = u.Email,
                username = u.Username,
                display_name = u.DisplayName,
                role = u.Role.ToString().ToLowerInvariant(),
                status = u.Status.ToString().ToLowerInvariant()
            };
        }

        [Serializable]
        class ProfileDto
        {
            public string user_id;
            public string bio;
            public string country;
            public string timezone;
            public string profile_image_id;
            public string avatar_url;

            public UserProfile ToModel() => new UserProfile
            {
                UserId = user_id,
                Bio = bio,
                Country = country,
                Timezone = timezone,
                ProfileImageId = profile_image_id,
                ProfileImageUrl = avatar_url
            };

            public static ProfileDto From(UserProfile p) => new ProfileDto
            {
                user_id = p.UserId,
                bio = p.Bio,
                country = p.Country,
                timezone = p.Timezone,
                profile_image_id = p.ProfileImageId,
                avatar_url = p.ProfileImageUrl
            };
        }

        [Serializable]
        class PrivacyDto
        {
            public string user_id;
            public bool show_profile;
            public bool show_schedule;
            public bool show_location;
            public bool show_assignments;
            public bool show_online_status;
            public bool show_friends;

            public PrivacySettings ToModel() => new PrivacySettings
            {
                UserId = user_id,
                ShowProfile = show_profile,
                ShowSchedule = show_schedule,
                ShowLocation = show_location,
                ShowAssignments = show_assignments,
                ShowOnlineStatus = show_online_status,
                ShowFriends = show_friends
            };

            public static PrivacyDto From(PrivacySettings s) => new PrivacyDto
            {
                user_id = s.UserId,
                show_profile = s.ShowProfile,
                show_schedule = s.ShowSchedule,
                show_location = s.ShowLocation,
                show_assignments = s.ShowAssignments,
                show_online_status = s.ShowOnlineStatus,
                show_friends = s.ShowFriends
            };
        }

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
