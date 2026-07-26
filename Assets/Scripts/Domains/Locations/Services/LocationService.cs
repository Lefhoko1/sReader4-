using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Domains.Identity.Repositories;
using SReader.Domains.Locations.Models;
using SReader.Domains.Locations.Repositories;

namespace SReader.Domains.Locations.Services
{
    public sealed class LocationService : ILocationService
    {
        readonly IUserLocationRepository repository;
        readonly IUserRepository userRepository;
        readonly CurrentSessionHolder sessionHolder;

        public LocationService(IUserLocationRepository repository, IUserRepository userRepository, CurrentSessionHolder sessionHolder)
        {
            this.repository     = Guard.NotNull(repository, nameof(repository));
            this.userRepository = Guard.NotNull(userRepository, nameof(userRepository));
            this.sessionHolder  = Guard.NotNull(sessionHolder, nameof(sessionHolder));
        }

        public Task<Result<UserLocation>> GetMyLocationAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<UserLocation>("Not signed in."));
            return repository.GetForUserAsync(sessionHolder.CurrentUserId, ct);
        }

        public async Task<Result<UserLocation>> GetVisibleLocationAsync(string ownerUserId, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Result.Fail<UserLocation>("Not signed in.");

            var privacy = await userRepository.GetPrivacySettingsAsync(ownerUserId, ct);
            if (privacy.IsFailure) return Result.Fail<UserLocation>(privacy.Error);
            if (privacy.Value != null && !privacy.Value.ShowLocation)
                return Result.Fail<UserLocation>("This user's location is private.");

            return await repository.GetForUserAsync(ownerUserId, ct);
        }

        public Task<Result> UpdateMyLocationAsync(UserLocation location, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            if (location == null)
                return Task.FromResult(Result.Fail("Location is required."));

            location.UserId = sessionHolder.CurrentUserId;
            return repository.UpsertAsync(location, ct);
        }

        public Task<Result> ClearMyLocationAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail("Not signed in."));
            return repository.DeleteForUserAsync(sessionHolder.CurrentUserId, ct);
        }
    }
}
