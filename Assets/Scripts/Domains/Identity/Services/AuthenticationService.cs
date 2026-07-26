using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Events;
using SReader.Core.Logging;
using SReader.Domains.Identity.Events;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Repositories;

namespace SReader.Domains.Identity.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        readonly IAuthenticationRepository repository;
        readonly ISessionStore sessionStore;
        readonly CurrentSessionHolder sessionHolder;
        readonly PasswordResetFlow resetFlow;
        readonly IEventBus eventBus;
        readonly IAppLogger logger;
        readonly IClock clock;
        readonly IConnectivity connectivity; // optional; null ⇒ assume online

        public AuthenticationService(
            IAuthenticationRepository repository,
            ISessionStore sessionStore,
            CurrentSessionHolder sessionHolder,
            PasswordResetFlow resetFlow,
            IEventBus eventBus,
            IAppLogger logger,
            IClock clock,
            IConnectivity connectivity = null)
        {
            this.repository    = Guard.NotNull(repository, nameof(repository));
            this.sessionStore  = Guard.NotNull(sessionStore, nameof(sessionStore));
            this.sessionHolder = Guard.NotNull(sessionHolder, nameof(sessionHolder));
            this.resetFlow     = Guard.NotNull(resetFlow, nameof(resetFlow));
            this.eventBus      = Guard.NotNull(eventBus, nameof(eventBus));
            this.logger        = Guard.NotNull(logger, nameof(logger));
            this.clock         = Guard.NotNull(clock, nameof(clock));
            this.connectivity  = connectivity;
        }

        public async Task<Result> RegisterAsync(string email, string password, string firstName, string lastName, UserRole role, CancellationToken ct = default)
        {
            email = email?.Trim() ?? "";
            if (!IdentityValidation.IsValidEmail(email))
                return Result.Fail("Please enter a valid email address.");
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                return Result.Fail("Please enter your first and last name.");

            var passwordCheck = IdentityValidation.ValidatePassword(password);
            if (passwordCheck.IsFailure) return passwordCheck;

            var displayName = $"{firstName.Trim()} {lastName.Trim()}";
            var result = await repository.RegisterAsync(email, password, displayName, role, ct);
            if (result.IsFailure) return result;

            await AdoptSessionAsync(result.Value);
            eventBus.Publish(new UserRegisteredEvent(result.Value.UserId, email));
            logger.Info($"Registered new user {result.Value.UserId}");
            return Result.Ok();
        }

        public async Task<Result> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            email = email?.Trim() ?? "";
            if (!IdentityValidation.IsValidEmail(email))
                return Result.Fail("Please enter a valid email address.");
            if (string.IsNullOrEmpty(password))
                return Result.Fail("Please enter your password.");

            var result = await repository.LoginAsync(email, password, ct);
            if (result.IsFailure) return result;

            await AdoptSessionAsync(result.Value);
            eventBus.Publish(new UserLoggedInEvent(result.Value.UserId));
            logger.Info($"User {result.Value.UserId} logged in");
            return Result.Ok();
        }

        public async Task<Result> LogoutAsync(CancellationToken ct = default)
        {
            var session = sessionHolder.Session;
            if (session == null) return Result.Ok();

            var result = await repository.LogoutAsync(session.AccessToken, ct);

            // Local sign-out always succeeds even if the server call failed —
            // the user must never be trapped in a signed-in state offline.
            sessionHolder.Clear();
            await sessionStore.ClearAsync();
            eventBus.Publish(new UserLoggedOutEvent(session.UserId));

            if (result.IsFailure)
                logger.Warning($"Server logout failed (signed out locally): {result.Error}");
            return Result.Ok();
        }

        public async Task<Result> RestoreSessionAsync(CancellationToken ct = default)
        {
            var stored = await sessionStore.LoadAsync();
            if (stored == null) return Result.Fail("No saved session.");

            // Access token still valid → restore straight from local storage (works offline).
            if (!stored.IsExpired(clock.UtcNow))
            {
                sessionHolder.Set(stored);
                eventBus.Publish(new UserLoggedInEvent(stored.UserId));
                return Result.Ok();
            }

            // Access token expired. Only a real refresh against the server can renew it,
            // so attempt it when we believe we have a connection.
            bool online = connectivity == null || connectivity.IsOnline;
            if (online)
            {
                var refreshed = await repository.RefreshSessionAsync(stored.RefreshToken, ct);
                if (refreshed.IsSuccess)
                {
                    await AdoptSessionAsync(refreshed.Value);
                    eventBus.Publish(new UserLoggedInEvent(refreshed.Value.UserId));
                    return Result.Ok();
                }

                // Still online after the attempt ⇒ the server genuinely rejected the
                // refresh token ⇒ the session really is dead, so sign out.
                if (connectivity == null || connectivity.IsOnline)
                {
                    await sessionStore.ClearAsync();
                    return Result.Fail("Session expired — please sign in again.");
                }
            }

            // Offline (or we dropped offline mid-refresh): keep the user signed in
            // locally so they can still open the app and view their cached data. The
            // token is renewed automatically on the next launch with a connection.
            sessionHolder.Set(stored);
            eventBus.Publish(new UserLoggedInEvent(stored.UserId));
            logger.Info("Restored an expired session offline — will refresh when back online.");
            return Result.Ok();
        }

        public async Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default)
        {
            email = email?.Trim() ?? "";

            // Empty email on an active flow = "resend the code" from the OTP page.
            if (email.Length == 0 && resetFlow.HasEmail)
                email = resetFlow.Email;

            if (!IdentityValidation.IsValidEmail(email))
                return Result.Fail("Please enter a valid email address.");

            var result = await repository.RequestPasswordResetAsync(email, ct);
            if (result.IsFailure) return result;

            resetFlow.Begin(email);
            return Result.Ok();
        }

        public async Task<Result> VerifyResetCodeAsync(string code, CancellationToken ct = default)
        {
            if (!resetFlow.HasEmail)
                return Result.Fail("Start by requesting a reset code first.");
            if (string.IsNullOrEmpty(code) || code.Length < IdentityValidation.OtpLength)
                return Result.Fail($"Please enter all {IdentityValidation.OtpLength} digits.");

            var result = await repository.VerifyPasswordResetCodeAsync(resetFlow.Email, code, ct);
            if (result.IsFailure) return result;

            resetFlow.MarkCodeVerified(code);
            return Result.Ok();
        }

        public async Task<Result> CompletePasswordResetAsync(string newPassword, CancellationToken ct = default)
        {
            if (!resetFlow.IsCodeVerified)
                return Result.Fail("Verify the code from your email first.");

            var passwordCheck = IdentityValidation.ValidatePassword(newPassword);
            if (passwordCheck.IsFailure) return passwordCheck;

            var result = await repository.ResetPasswordAsync(resetFlow.Email, resetFlow.VerifiedCode, newPassword, ct);
            if (result.IsFailure) return result;

            resetFlow.Complete();
            return Result.Ok();
        }

        async Task AdoptSessionAsync(AuthSession session)
        {
            sessionHolder.Set(session);
            await sessionStore.SaveAsync(session);
        }
    }
}
