using System.Threading;
using System.Threading.Tasks;
using SQLite;
using SReader.Core.Common;
using SReader.Domains.Sync.Repositories;
using UnityEngine;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>One key/value snapshot row in the offline read cache.</summary>
    [Table("cache_entries")]
    public sealed class CacheEntryRow
    {
        [PrimaryKey] public string Key { get; set; }
        public string Json { get; set; }
    }

    /// <summary>
    /// Offline read cache (CachedAssignments / CachedLessons / CachedProfiles /
    /// CachedFriends / CachedImages). Entries are JSON snapshots keyed by
    /// "entityType:id", now persisted to a real SQLite table so the cache
    /// survives app restarts.
    /// </summary>
    public sealed class SqliteLocalCache : ILocalCache
    {
        readonly SqliteDatabase db;

        public SqliteLocalCache(SqliteDatabase db)
        {
            this.db = Guard.NotNull(db, nameof(db));
        }

        public Task<Result> PutAsync<T>(string key, T value, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(key)) return Task.FromResult(Result.Fail("Cache key is required."));
            if (value == null) return Task.FromResult(Result.Fail("Cache value is required."));

            var json = JsonUtility.ToJson(value);
            lock (db.Gate) db.Connection.InsertOrReplace(new CacheEntryRow { Key = key, Json = json });
            return Task.FromResult(Result.Ok());
        }

        public Task<Result<T>> GetAsync<T>(string key, CancellationToken ct = default)
        {
            CacheEntryRow row;
            lock (db.Gate) row = db.Connection.Find<CacheEntryRow>(key);
            if (row == null) return Task.FromResult(Result.Fail<T>($"No cached entry for '{key}'."));
            return Task.FromResult(Result.Ok(JsonUtility.FromJson<T>(row.Json)));
        }

        public Task<Result> RemoveAsync(string key, CancellationToken ct = default)
        {
            lock (db.Gate) db.Connection.Delete<CacheEntryRow>(key);
            return Task.FromResult(Result.Ok());
        }
    }
}
