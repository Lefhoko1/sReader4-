using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Sync.Models;
using SReader.Domains.Sync.Repositories;

namespace SReader.Domains.Sync.Services
{
    public sealed class SyncService : ISyncService
    {
        const int BatchSize = 25;
        const int MaxRetries = 5;

        readonly ISyncQueueRepository queue;
        readonly IAppLogger logger;
        readonly IClock clock;

        public bool IsSyncing { get; private set; }

        public SyncService(ISyncQueueRepository queue, IAppLogger logger, IClock clock)
        {
            this.queue  = Guard.NotNull(queue, nameof(queue));
            this.logger = Guard.NotNull(logger, nameof(logger));
            this.clock  = Guard.NotNull(clock, nameof(clock));
        }

        public Task<Result> QueueChangeAsync(SyncOperationType type, string entityType, string entityId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(entityId))
                return Task.FromResult(Result.Fail("Entity type and id are required."));

            return queue.EnqueueAsync(new SyncOperation
            {
                OperationType = type,
                EntityType = entityType,
                EntityId = entityId,
                CreatedAt = clock.UtcNow,
                Status = SyncStatus.Pending
            }, ct);
        }

        public async Task<Result> SyncNowAsync(CancellationToken ct = default)
        {
            if (IsSyncing) return Result.Fail("A sync is already running.");
            IsSyncing = true;

            try
            {
                var pending = await queue.GetPendingAsync(BatchSize, ct);
                if (pending.IsFailure) return pending;

                foreach (var op in pending.Value)
                {
                    if (ct.IsCancellationRequested) break;

                    await queue.UpdateStatusAsync(op.Id, SyncStatus.InProgress, ct);

                    // TODO Phase 7: dispatch by op.EntityType to the owning
                    // domain repository (upload/download/delete) and apply
                    // last-write-wins conflict resolution on version clash.
                    var processed = Result.Fail("Sync processing is not implemented yet (Phase 7).");

                    if (processed.IsSuccess)
                    {
                        await queue.UpdateStatusAsync(op.Id, SyncStatus.Completed, ct);
                    }
                    else
                    {
                        var status = op.RetryCount + 1 >= MaxRetries ? SyncStatus.Failed : SyncStatus.Pending;
                        await queue.UpdateStatusAsync(op.Id, status, ct);
                        logger.Warning($"Sync op {op.Id} ({op.EntityType}/{op.EntityId}) deferred: {processed.Error}");
                    }
                }

                await queue.RemoveCompletedAsync(ct);
                return Result.Ok();
            }
            finally
            {
                IsSyncing = false;
            }
        }
    }
}
