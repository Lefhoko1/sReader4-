using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;
using SReader.Domains.Sync.Models;
using SReader.Domains.Sync.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// Volatile <see cref="ILocalCache"/> used when the native sqlite3 library
    /// isn't available (so the app still runs, just without a persistent cache).
    /// </summary>
    public sealed class InMemoryLocalCache : ILocalCache
    {
        readonly Dictionary<string, string> store = new Dictionary<string, string>();
        readonly object gate = new object();

        public Task<Result> PutAsync<T>(string key, T value, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(key)) return Task.FromResult(Result.Fail("Cache key is required."));
            if (value == null) return Task.FromResult(Result.Fail("Cache value is required."));

            var json = JsonUtility.ToJson(value);
            lock (gate) store[key] = json;
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<T>> GetAsync<T>(string key, CancellationToken ct = default)
        {
            string json;
            lock (gate)
            {
                if (!store.TryGetValue(key, out json))
                    return Task.FromResult(Result.Fail<T>($"No cached entry for '{key}'."));
            }
            return Task.FromResult(Result.Ok(JsonUtility.FromJson<T>(json)));
        }

        public Task<Result> RemoveAsync(string key, CancellationToken ct = default)
        {
            lock (gate) store.Remove(key);
            return Task.FromResult(Result.Ok());
        }
    }

    /// <summary>
    /// Volatile <see cref="ISyncQueueRepository"/> used when the native sqlite3
    /// library isn't available. Queued changes live only for the session.
    /// </summary>
    public sealed class InMemorySyncQueueRepository : ISyncQueueRepository
    {
        readonly List<SyncOperation> operations = new List<SyncOperation>();
        readonly object gate = new object();

        public Task<Result> EnqueueAsync(SyncOperation operation, CancellationToken ct = default)
        {
            if (operation == null) return Task.FromResult(Result.Fail("Operation is required."));
            if (string.IsNullOrEmpty(operation.Id)) operation.Id = Guid.NewGuid().ToString("N");

            lock (gate) operations.Add(operation);
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<IReadOnlyList<SyncOperation>>> GetPendingAsync(int limit, CancellationToken ct = default)
        {
            lock (gate)
            {
                IReadOnlyList<SyncOperation> pending = operations
                    .Where(o => o.Status == SyncStatus.Pending)
                    .OrderBy(o => o.CreatedAt)
                    .Take(limit)
                    .ToList();
                return Task.FromResult(Result.Ok(pending));
            }
        }

        public Task<Result> UpdateStatusAsync(string operationId, SyncStatus status, CancellationToken ct = default)
        {
            lock (gate)
            {
                var op = operations.FirstOrDefault(o => o.Id == operationId);
                if (op == null) return Task.FromResult(Result.Fail($"Sync operation '{operationId}' not found."));

                if (status == SyncStatus.Pending && op.Status == SyncStatus.InProgress)
                    op.RetryCount++;
                op.Status = status;
                return Task.FromResult(Result.Ok());
            }
        }

        public Task<Result> RemoveCompletedAsync(CancellationToken ct = default)
        {
            lock (gate) operations.RemoveAll(o => o.Status == SyncStatus.Completed);
            return Task.FromResult(Result.Ok());
        }
    }

    /// <summary>
    /// Volatile <see cref="IGameProgressStore"/> used when the native sqlite3
    /// library isn't available. Resume points live only for the session.
    /// </summary>
    public sealed class InMemoryGameProgressStore : IGameProgressStore
    {
        readonly Dictionary<string, GameProgress> store = new Dictionary<string, GameProgress>();
        readonly object gate = new object();

        static string KeyOf(string userId, string assignmentId)
            => (string.IsNullOrEmpty(userId) ? "anon" : userId) + "|" + (assignmentId ?? "");

        public GameProgress Load(string userId, string assignmentId)
        {
            lock (gate) return store.TryGetValue(KeyOf(userId, assignmentId), out var p) ? p : null;
        }

        public void Save(GameProgress p)
        {
            if (p == null || string.IsNullOrEmpty(p.AssignmentId)) return;
            lock (gate) store[KeyOf(p.UserId, p.AssignmentId)] = p;
        }

        public void Clear(string userId, string assignmentId)
        {
            lock (gate) store.Remove(KeyOf(userId, assignmentId));
        }
    }
}
