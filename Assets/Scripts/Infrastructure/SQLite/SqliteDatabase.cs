using System;
using System.IO;
using SQLite;
using SReader.Core.Common;
using SReader.Core.Logging;
using UnityEngine;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// Owns the single on-disk SQLite connection (in Application.persistentDataPath)
    /// and creates every table on first run. sqlite-net's SQLiteConnection is NOT
    /// thread-safe, so every caller serializes access through <see cref="Gate"/>.
    /// Registered as a singleton in the composition root and shared by all the
    /// SQLite-backed repositories (cache, sync queue, academy mirror).
    /// </summary>
    public sealed class SqliteDatabase : IDisposable
    {
        /// <summary>Lock held around every Connection access (sqlite-net is single-threaded).</summary>
        public readonly object Gate = new object();

        public SQLiteConnection Connection { get; }

        public SqliteDatabase(string fileName, IAppLogger logger = null)
        {
            Guard.NotNullOrEmpty(fileName, nameof(fileName));

            var path = Path.Combine(Application.persistentDataPath, fileName);
            Connection = new SQLiteConnection(path);

            Connection.CreateTable<CacheEntryRow>();
            Connection.CreateTable<SyncOperationRow>();
            Connection.CreateTable<AcademyRow>();
            Connection.CreateTable<FriendshipRow>();
            Connection.CreateTable<AssignmentScheduleRow>();
            Connection.CreateTable<AssignmentAttemptRow>();
            Connection.CreateTable<AssignmentSubmissionRow>();
            Connection.CreateTable<SupabaseReadCacheRow>();
            Connection.CreateTable<OfflineCredentialRow>();
            Connection.CreateTable<GameProgressRow>();

            logger?.Info($"[SQLite] Database ready at {path}");
        }

        public void Dispose() => Connection?.Dispose();
    }
}
