using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Files.Models;
using SReader.Domains.Files.Repositories;

namespace SReader.Domains.Files.Services
{
    public sealed class FileVersioningService : IFileVersioningService
    {
        readonly IAssetVersionRepository repository;
        readonly IClock clock;

        public FileVersioningService(IAssetVersionRepository repository, IClock clock)
        {
            this.repository = Guard.NotNull(repository, nameof(repository));
            this.clock      = Guard.NotNull(clock, nameof(clock));
        }

        public async Task<Result<bool>> IsUpdateAvailableAsync(AssetType assetType, string assetId, int localVersion, CancellationToken ct = default)
        {
            var latest = await repository.GetLatestAsync(assetType, assetId, ct);
            if (latest.IsFailure) return Result.Fail<bool>(latest.Error);
            if (latest.Value == null) return Result.Ok(false);
            return Result.Ok(latest.Value.Version > localVersion);
        }

        public Task<Result<AssetVersion>> GetLatestVersionAsync(AssetType assetType, string assetId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(assetId))
                return Task.FromResult(Result.Fail<AssetVersion>("Asset id is required."));
            return repository.GetLatestAsync(assetType, assetId, ct);
        }

        public Task<Result> RegisterNewVersionAsync(AssetVersion version, CancellationToken ct = default)
        {
            if (version == null || string.IsNullOrEmpty(version.AssetId))
                return Task.FromResult(Result.Fail("An asset version with an asset id is required."));
            if (version.Version < 1)
                return Task.FromResult(Result.Fail("Version must start at 1."));

            version.CreatedAt = clock.UtcNow;
            return repository.RegisterVersionAsync(version, ct);
        }
    }
}
