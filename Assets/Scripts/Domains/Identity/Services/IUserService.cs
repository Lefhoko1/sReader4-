using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;

namespace SReader.Domains.Identity.Services
{
    public interface IUserService
    {
        Task<Result<User>> GetCurrentUserAsync(CancellationToken ct = default);
        Task<Result> UpdateUserAsync(User user, CancellationToken ct = default);
        Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken ct = default);
        Task<Result> UpdateProfileAsync(UserProfile profile, CancellationToken ct = default);
        Task<Result<PrivacySettings>> GetPrivacySettingsAsync(string userId, CancellationToken ct = default);
        Task<Result> UpdatePrivacySettingsAsync(PrivacySettings settings, CancellationToken ct = default);
    }
}
