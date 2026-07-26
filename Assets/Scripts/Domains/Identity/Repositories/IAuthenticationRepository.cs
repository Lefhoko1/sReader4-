using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;

namespace SReader.Domains.Identity.Repositories
{
    /// <summary>
    /// Auth gateway — implemented by Infrastructure.Supabase. The domain
    /// never sees HTTP, tokens formats, or Supabase types beyond AuthSession.
    /// </summary>
    public interface IAuthenticationRepository
    {
        Task<Result<AuthSession>> RegisterAsync(string email, string password, string displayName, UserRole role, CancellationToken ct = default);
        Task<Result<AuthSession>> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<Result> LogoutAsync(string accessToken, CancellationToken ct = default);
        Task<Result<AuthSession>> RefreshSessionAsync(string refreshToken, CancellationToken ct = default);

        Task<Result> RequestPasswordResetAsync(string email, CancellationToken ct = default);
        Task<Result> VerifyPasswordResetCodeAsync(string email, string code, CancellationToken ct = default);
        Task<Result> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken ct = default);
    }
}
