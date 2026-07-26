using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Repositories;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// Wraps the real (Supabase) auth gateway with on-device offline login.
    ///
    /// • Online sign-in / registration goes to the server as usual, and on
    ///   success the email + password (hashed) and the session are saved to
    ///   SQLite so the same login works later without a connection.
    /// • When the device is offline — or an online attempt fails because the
    ///   network couldn't be reached — the email + password are verified
    ///   against the saved credential and the stored session is returned, so
    ///   the user can get in and work against their cached data.
    ///
    /// A genuine server rejection while online (e.g. wrong password) is passed
    /// straight through — we only fall back to local checks for connectivity
    /// failures, never to mask a real "no".
    /// </summary>
    public sealed class OfflineAuthenticationRepository : IAuthenticationRepository
    {
        readonly IAuthenticationRepository inner;
        readonly OfflineCredentialStore credentials;
        readonly IConnectivity connectivity;   // null ⇒ assume online
        readonly IAppLogger logger;

        public OfflineAuthenticationRepository(
            IAuthenticationRepository inner,
            OfflineCredentialStore credentials,
            IConnectivity connectivity = null,
            IAppLogger logger = null)
        {
            this.inner        = Guard.NotNull(inner, nameof(inner));
            this.credentials  = Guard.NotNull(credentials, nameof(credentials));
            this.connectivity = connectivity;
            this.logger       = logger;
        }

        bool Online => connectivity == null || connectivity.IsOnline;

        public async Task<Result<AuthSession>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            if (Online)
            {
                var result = await inner.LoginAsync(email, password, ct);
                if (result.IsSuccess)
                {
                    // Remember this login so it works offline next time.
                    credentials.Save(email, password, result.Value);
                    return result;
                }

                // A real answer from the server (bad credentials, unconfirmed
                // email, …) must be shown as-is — only retry locally if we
                // simply couldn't reach the network.
                if (!IsNetworkError(result.Error))
                    return result;

                logger?.Warning("[OfflineAuth] Online sign-in couldn't reach the server — trying saved offline login.");
            }

            var session = credentials.Verify(email, password);
            if (session != null)
            {
                logger?.Info("[OfflineAuth] Signed in offline using saved credentials.");
                return Result.Ok(session);
            }

            return Result.Fail<AuthSession>(Online
                ? "Couldn't reach the server, and no saved offline login matches this email and password."
                : "You're offline. Sign in online once on this device first to enable offline login.");
        }

        public async Task<Result<AuthSession>> RegisterAsync(string email, string password, string displayName, UserRole role, CancellationToken ct = default)
        {
            var result = await inner.RegisterAsync(email, password, displayName, role, ct);
            // Only sessions (auto-confirmed projects) yield a credential to save;
            // "confirm your email" registrations return a failure and are skipped.
            if (result.IsSuccess)
                credentials.Save(email, password, result.Value);
            return result;
        }

        // Logout, refresh and the password-reset flow are inherently online —
        // pass them straight through. After a password reset the next online
        // sign-in re-saves the new password for offline use automatically.
        public Task<Result> LogoutAsync(string accessToken, CancellationToken ct = default)
            => inner.LogoutAsync(accessToken, ct);

        public Task<Result<AuthSession>> RefreshSessionAsync(string refreshToken, CancellationToken ct = default)
            => inner.RefreshSessionAsync(refreshToken, ct);

        public Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default)
            => inner.RequestPasswordResetAsync(email, ct);

        public Task<Result> VerifyPasswordResetCodeAsync(string email, string code, CancellationToken ct = default)
            => inner.VerifyPasswordResetCodeAsync(email, code, ct);

        public Task<Result> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken ct = default)
            => inner.ResetPasswordAsync(email, code, newPassword, ct);

        // SupabaseHttp surfaces connectivity failures as "Network error: …"
        // (HTTP statuses like 400/403 are returned as their real message).
        static bool IsNetworkError(string error)
            => !string.IsNullOrEmpty(error) &&
               error.IndexOf("Network error", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
