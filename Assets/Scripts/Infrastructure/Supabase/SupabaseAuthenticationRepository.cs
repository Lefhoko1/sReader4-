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
    /// Supabase Auth (GoTrue) over REST — {SupabaseUrl}/auth/v1.
    /// The 6-digit OTP flow requires the "Reset Password" email template in
    /// the Supabase dashboard to include {{ .Token }} (by default it only
    /// contains a link).
    /// </summary>
    public sealed class SupabaseAuthenticationRepository : SupabaseRepositoryBase, IAuthenticationRepository
    {
        // Session minted by the recovery-code verify step; consumed by ResetPasswordAsync.
        AuthSession recoverySession;

        string AuthUrl => Settings.SupabaseUrl.TrimEnd('/') + "/auth/v1";

        public SupabaseAuthenticationRepository(AppSettings settings) : base(settings) { }

        public async Task<Result<AuthSession>> RegisterAsync(string email, string password, string displayName, UserRole role, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AuthSession>(configured.Error);

            var body = JsonUtility.ToJson(new SignupRequest
            {
                email = email,
                password = password,
                // role rides in the signup metadata so the handle_new_user DB
                // trigger can seed public.users.role at account creation.
                data = new SignupMetadata { display_name = displayName, role = role.ToString().ToLowerInvariant() }
            });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{AuthUrl}/signup",
                Settings.SupabaseAnonKey, body, ct: ct);
            if (response.IsFailure) return Result.Fail<AuthSession>(response.Error);

            var session = ParseSession(response.Value);
            if (session == null)
            {
                // "Confirm email" is enabled on the project: the account exists
                // but there is no session until the user clicks the email link.
                return Result.Fail<AuthSession>(
                    "Account created — check your inbox to confirm your email, then sign in.");
            }
            return Result.Ok(session);
        }

        public async Task<Result<AuthSession>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AuthSession>(configured.Error);

            var body = JsonUtility.ToJson(new PasswordGrantRequest { email = email, password = password });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post,
                $"{AuthUrl}/token?grant_type=password", Settings.SupabaseAnonKey, body, ct: ct);
            if (response.IsFailure) return Result.Fail<AuthSession>(response.Error);

            var session = ParseSession(response.Value);
            return session != null
                ? Result.Ok(session)
                : Result.Fail<AuthSession>("Sign-in succeeded but no session was returned.");
        }

        public async Task<Result> LogoutAsync(string accessToken, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{AuthUrl}/logout",
                Settings.SupabaseAnonKey, "{}", accessToken, ct);
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result<AuthSession>> RefreshSessionAsync(string refreshToken, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return Result.Fail<AuthSession>(configured.Error);

            var body = JsonUtility.ToJson(new RefreshGrantRequest { refresh_token = refreshToken });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post,
                $"{AuthUrl}/token?grant_type=refresh_token", Settings.SupabaseAnonKey, body, ct: ct);
            if (response.IsFailure) return Result.Fail<AuthSession>(response.Error);

            var session = ParseSession(response.Value);
            return session != null
                ? Result.Ok(session)
                : Result.Fail<AuthSession>("Session refresh returned no session.");
        }

        public async Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new RecoverRequest { email = email });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{AuthUrl}/recover",
                Settings.SupabaseAnonKey, body, ct: ct);
            return response.IsSuccess ? Result.Ok() : Result.Fail(response.Error);
        }

        public async Task<Result> VerifyPasswordResetCodeAsync(string email, string code, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            var body = JsonUtility.ToJson(new VerifyRequest { type = "recovery", email = email, token = code });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, $"{AuthUrl}/verify",
                Settings.SupabaseAnonKey, body, ct: ct);
            if (response.IsFailure) return Result.Fail(response.Error);

            recoverySession = ParseSession(response.Value);
            return recoverySession != null
                ? Result.Ok()
                : Result.Fail("The code was accepted but no recovery session was returned.");
        }

        public async Task<Result> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken ct = default)
        {
            var configured = CheckConfigured();
            if (configured.IsFailure) return configured;

            if (recoverySession == null)
                return Result.Fail("Verify the code from your email first.");

            var body = JsonUtility.ToJson(new UpdatePasswordRequest { password = newPassword });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Put, $"{AuthUrl}/user",
                Settings.SupabaseAnonKey, body, recoverySession.AccessToken, ct);
            if (response.IsFailure) return Result.Fail(response.Error);

            recoverySession = null; // single-use
            return Result.Ok();
        }

        // ── GoTrue payloads (JsonUtility needs public fields) ──

        [Serializable] class SignupMetadata { public string display_name; public string role; }
        [Serializable] class SignupRequest { public string email; public string password; public SignupMetadata data; }
        [Serializable] class PasswordGrantRequest { public string email; public string password; }
        [Serializable] class RefreshGrantRequest { public string refresh_token; }
        [Serializable] class RecoverRequest { public string email; }
        [Serializable] class VerifyRequest { public string type; public string email; public string token; }
        [Serializable] class UpdatePasswordRequest { public string password; }

        [Serializable] class UserDto { public string id; public string email; }
        [Serializable]
        class SessionResponse
        {
            public string access_token;
            public string refresh_token;
            public int expires_in;
            public UserDto user;
        }

        static AuthSession ParseSession(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            SessionResponse dto;
            try { dto = JsonUtility.FromJson<SessionResponse>(json); }
            catch { return null; }

            if (dto == null || string.IsNullOrEmpty(dto.access_token)) return null;

            return new AuthSession
            {
                UserId = dto.user != null ? dto.user.id : null,
                Email = dto.user != null ? dto.user.email : null,
                AccessToken = dto.access_token,
                RefreshToken = dto.refresh_token,
                ExpiresAtUtc = DateTime.UtcNow.AddSeconds(dto.expires_in > 0 ? dto.expires_in : 3600)
            };
        }
    }
}
