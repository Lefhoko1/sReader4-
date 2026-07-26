using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Files.Models;

namespace SReader.Domains.Files.Services
{
    public interface IFileVersioningService
    {
        /// <summary>True when the remote version is newer than the locally cached one.</summary>
        Task<Result<bool>> IsUpdateAvailableAsync(AssetType assetType, string assetId, int localVersion, CancellationToken ct = default);

        Task<Result<AssetVersion>> GetLatestVersionAsync(AssetType assetType, string assetId, CancellationToken ct = default);
        Task<Result> RegisterNewVersionAsync(AssetVersion version, CancellationToken ct = default);
    }
}
