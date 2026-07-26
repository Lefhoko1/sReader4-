using System;
using SQLite;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Infrastructure.Supabase;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>One cached Supabase GET response, keyed "{userId}|{url}".</summary>
    [Table("supabase_read_cache")]
    public sealed class SupabaseReadCacheRow
    {
        [PrimaryKey] public string Key { get; set; }
        public string Json { get; set; }
        public long CachedAtTicks { get; set; }
    }

    /// <summary>
    /// SQLite-backed <see cref="ISupabaseReadCache"/>. Entries are scoped to the
    /// signed-in user so one device shared by two accounts never serves one
    /// user's rows to the other (RLS-filtered URLs look identical otherwise).
    /// </summary>
    public sealed class SqliteSupabaseReadCache : ISupabaseReadCache
    {
        readonly SqliteDatabase db;
        readonly CurrentSessionHolder session;

        public SqliteSupabaseReadCache(SqliteDatabase db, CurrentSessionHolder session)
        {
            this.db      = Guard.NotNull(db, nameof(db));
            this.session = Guard.NotNull(session, nameof(session));
        }

        string KeyFor(string url) => (session.CurrentUserId ?? "anon") + "|" + url;

        public void Store(string url, string json)
        {
            if (string.IsNullOrEmpty(url)) return;
            var row = new SupabaseReadCacheRow
            {
                Key = KeyFor(url),
                Json = json ?? "",
                CachedAtTicks = DateTime.UtcNow.Ticks
            };
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        public bool TryGet(string url, out string json)
        {
            json = null;
            if (string.IsNullOrEmpty(url)) return false;

            SupabaseReadCacheRow row;
            lock (db.Gate) row = db.Connection.Find<SupabaseReadCacheRow>(KeyFor(url));
            if (row == null) return false;

            json = row.Json;
            return true;
        }
    }
}
