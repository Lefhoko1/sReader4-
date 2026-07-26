using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Files.Models;

namespace SReader.Domains.Files.Repositories
{
    public interface IAssetVersionRepository
    {
        Task<Result<AssetVersion>> GetLatestAsync(AssetType assetType, string assetId, CancellationToken ct = default);
        Task<Result> RegisterVersionAsync(AssetVersion version, CancellationToken ct = default);
    }
}
