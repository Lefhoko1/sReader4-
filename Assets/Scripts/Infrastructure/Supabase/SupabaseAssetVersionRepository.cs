using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Files.Models;
using SReader.Domains.Files.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>TODO Phase 7: asset_versions table + Supabase Storage buckets.</summary>
    public sealed class SupabaseAssetVersionRepository : SupabaseRepositoryBase, IAssetVersionRepository
    {
        public SupabaseAssetVersionRepository(AppSettings settings) : base(settings) { }

        public Task<Result<AssetVersion>> GetLatestAsync(AssetType assetType, string assetId, CancellationToken ct = default)
            => TodoAsync<AssetVersion>("Load latest asset version");

        public Task<Result> RegisterVersionAsync(AssetVersion version, CancellationToken ct = default)
            => TodoAsync("Register asset version");
    }
}
