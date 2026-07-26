using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;

namespace SReader.Domains.Identity.Repositories
{
    public interface IUserRepository
    {
        Task<Result<User>> GetByIdAsync(string userId, CancellationToken ct = default);
        Task<Result<User>> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<Result> UpdateAsync(User user, CancellationToken ct = default);

        Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertProfileAsync(UserProfile profile, CancellationToken ct = default);

        Task<Result<PrivacySettings>> GetPrivacySettingsAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertPrivacySettingsAsync(PrivacySettings settings, CancellationToken ct = default);
    }
}
