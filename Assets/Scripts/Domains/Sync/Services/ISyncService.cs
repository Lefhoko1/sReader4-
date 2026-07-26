using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Sync.Models;

namespace SReader.Domains.Sync.Services
{
    /// <summary>
    /// Owns all cloud↔local synchronization. The UI never syncs; it only
    /// reads through services that consult the cache when offline.
    /// </summary>
    public interface ISyncService
    {
        bool IsSyncing { get; }

        /// <summary>Record a local change to be pushed when online.</summary>
        Task<Result> QueueChangeAsync(SyncOperationType type, string entityType, string entityId, CancellationToken ct = default);

        /// <summary>Process the pending queue: upload changes, download updates, resolve conflicts.</summary>
        Task<Result> SyncNowAsync(CancellationToken ct = default);
    }
}
