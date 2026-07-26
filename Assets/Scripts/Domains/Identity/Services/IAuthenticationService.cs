using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;

namespace SReader.Domains.Identity.Services
{
    /// <summary>
    /// Application service behind every auth screen:
    /// Login → LoginAsync, Register → RegisterAsync,
    /// ForgotPassword → RequestPasswordResetAsync, OTP → VerifyResetCodeAsync,
    /// ResetPassword → CompletePasswordResetAsync, app start → RestoreSessionAsync.
    /// </summary>
    public interface IAuthenticationService
    {
        Task<Result> RegisterAsync(string email, string password, string firstName, string lastName, UserRole role, CancellationToken ct = default);
        Task<Result> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<Result> LogoutAsync(CancellationToken ct = default);

        /// <summary>Tries to resume the previous session from local storage.</summary>
        Task<Result> RestoreSessionAsync(CancellationToken ct = default);

        Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default);
        Task<Result> VerifyResetCodeAsync(string code, CancellationToken ct = default);
        Task<Result> CompletePasswordResetAsync(string newPassword, CancellationToken ct = default);
    }
}
