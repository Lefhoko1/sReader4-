using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Repositories;

namespace SReader.Domains.Identity.Services
{
    public sealed class UserService : IUserService
    {
        readonly IUserRepository repository;
        readonly CurrentSessionHolder sessionHolder;
        readonly IAppLogger logger;

        public UserService(IUserRepository repository, CurrentSessionHolder sessionHolder, IAppLogger logger)
        {
            this.repository    = Guard.NotNull(repository, nameof(repository));
            this.sessionHolder = Guard.NotNull(sessionHolder, nameof(sessionHolder));
            this.logger        = Guard.NotNull(logger, nameof(logger));
        }

        public Task<Result<User>> GetCurrentUserAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<User>("Not signed in."));
            return repository.GetByIdAsync(sessionHolder.CurrentUserId, ct);
        }

        public Task<Result> UpdateUserAsync(User user, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (user == null || string.IsNullOrEmpty(user.Id))
                return Task.FromResult(Result.Fail("A user with an id is required."));
            // A user may only edit their own account.
            if (user.Id != sessionHolder.CurrentUserId)
                return Task.FromResult(Result.Fail("You can only edit your own account."));
            return repository.UpdateAsync(user, ct);
        }

        public Task<Result<UserProfile>> GetProfileAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(Result.Fail<UserProfile>("User id is required."));
            return repository.GetProfileAsync(userId, ct);
        }

        public Task<Result> UpdateProfileAsync(UserProfile profile, CancellationToken ct = default)
        {
            if (profile == null || string.IsNullOrEmpty(profile.UserId))
                return Task.FromResult(Result.Fail("A profile with a user id is required."));
            return repository.UpsertProfileAsync(profile, ct);
        }

        public Task<Result<PrivacySettings>> GetPrivacySettingsAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult(Result.Fail<PrivacySettings>("User id is required."));
            return repository.GetPrivacySettingsAsync(userId, ct);
        }

        public Task<Result> UpdatePrivacySettingsAsync(PrivacySettings settings, CancellationToken ct = default)
        {
            if (settings == null || string.IsNullOrEmpty(settings.UserId))
                return Task.FromResult(Result.Fail("Privacy settings with a user id are required."));
            return repository.UpsertPrivacySettingsAsync(settings, ct);
        }
    }
}
