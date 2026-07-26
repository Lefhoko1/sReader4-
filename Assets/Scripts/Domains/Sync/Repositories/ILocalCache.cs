using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Sync.Repositories
{
    /// <summary>
    /// Offline read cache (SQLite: CachedAssignments, CachedLessons,
    /// CachedProfiles, CachedFriends, CachedImages). SQLite is a cache,
    /// not a mirror — entries are keyed snapshots, not relational data.
    /// </summary>
    public interface ILocalCache
    {
        Task<Result> PutAsync<T>(string key, T value, CancellationToken ct = default);
        Task<Result<T>> GetAsync<T>(string key, CancellationToken ct = default);
        Task<Result> RemoveAsync(string key, CancellationToken ct = default);
    }
}
