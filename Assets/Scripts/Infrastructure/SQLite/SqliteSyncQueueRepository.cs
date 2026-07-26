using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using SReader.Core.Common;
using SReader.Domains.Sync.Models;
using SReader.Domains.Sync.Repositories;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>One queued offline change, flattened for storage. Enums are
    /// stored as ints and the timestamp as ticks so sqlite-net can map them.</summary>
    [Table("sync_operations")]
    public sealed class SyncOperationRow
    {
        [PrimaryKey] public string Id { get; set; }
        public int OperationType { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public long CreatedAtTicks { get; set; }
        public int Status { get; set; }
        public int RetryCount { get; set; }

        public static SyncOperationRow From(SyncOperation o) => new SyncOperationRow
        {
            Id             = o.Id,
            OperationType  = (int)o.OperationType,
            EntityType     = o.EntityType,
            EntityId       = o.EntityId,
            CreatedAtTicks = o.CreatedAt.Ticks,
            Status         = (int)o.Status,
            RetryCount     = o.RetryCount
        };

        public SyncOperation ToModel() => new SyncOperation
        {
            Id            = Id,
            OperationType = (SyncOperationType)OperationType,
            EntityType    = EntityType,
            EntityId      = EntityId,
            CreatedAt     = new DateTime(CreatedAtTicks, DateTimeKind.Utc),
            Status        = (SyncStatus)Status,
            RetryCount    = RetryCount
        };
    }

    /// <summary>
    /// Sync queue (PendingSyncOperations), now persisted to a real SQLite table
    /// so queued offline changes survive an app restart and are pushed on the
    /// next online sync.
    /// </summary>
    public sealed class SqliteSyncQueueRepository : ISyncQueueRepository
    {
        readonly SqliteDatabase db;

        public SqliteSyncQueueRepository(SqliteDatabase db)
        {
            this.db = Guard.NotNull(db, nameof(db));
        }

        public Task<Result> EnqueueAsync(SyncOperation operation, CancellationToken ct = default)
        {
            if (operation == null) return Task.FromResult(Result.Fail("Operation is required."));
            if (string.IsNullOrEmpty(operation.Id)) operation.Id = Guid.NewGuid().ToString("N");

            lock (db.Gate) db.Connection.InsertOrReplace(SyncOperationRow.From(operation));
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<IReadOnlyList<SyncOperation>>> GetPendingAsync(int limit, CancellationToken ct = default)
        {
            var pendingStatus = (int)SyncStatus.Pending;
            List<SyncOperationRow> rows;
            lock (db.Gate)
            {
                rows = db.Connection.Table<SyncOperationRow>()
                    .Where(r => r.Status == pendingStatus)
                    .OrderBy(r => r.CreatedAtTicks)
                    .Take(limit)
                    .ToList();
            }

            IReadOnlyList<SyncOperation> pending = rows.Select(r => r.ToModel()).ToList();
            return Task.FromResult(Result.Ok(pending));
        }

        public Task<Result> UpdateStatusAsync(string operationId, SyncStatus status, CancellationToken ct = default)
        {
            lock (db.Gate)
            {
                var row = db.Connection.Find<SyncOperationRow>(operationId);
                if (row == null) return Task.FromResult(Result.Fail($"Sync operation '{operationId}' not found."));

                if (status == SyncStatus.Pending && row.Status == (int)SyncStatus.InProgress)
                    row.RetryCount++; // returned to the queue after a failed attempt
                row.Status = (int)status;
                db.Connection.Update(row);
                return Task.FromResult(Result.Ok());
            }
        }

        public Task<Result> RemoveCompletedAsync(CancellationToken ct = default)
        {
            var completed = (int)SyncStatus.Completed;
            lock (db.Gate)
                db.Connection.Execute("DELETE FROM sync_operations WHERE Status = ?", completed);
            return Task.FromResult(Result.Ok());
        }
    }
}
