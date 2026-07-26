using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Sync.Models;

namespace SReader.Domains.Sync.Repositories
{
    /// <summary>Backed by SQLite (PendingSyncOperations table) — never by Supabase.</summary>
    public interface ISyncQueueRepository
    {
        Task<Result> EnqueueAsync(SyncOperation operation, CancellationToken ct = default);
        Task<Result<IReadOnlyList<SyncOperation>>> GetPendingAsync(int limit, CancellationToken ct = default);
        Task<Result> UpdateStatusAsync(string operationId, SyncStatus status, CancellationToken ct = default);
        Task<Result> RemoveCompletedAsync(CancellationToken ct = default);
    }
}
