using System;
using System.Collections.Generic;
using SQLite;
using SReader.Core.Common;
using SReader.Domains.Assignments.Models;
using SReader.Domains.Assignments.Services;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>One saved game attempt on this device (the resume row).</summary>
    [Table("game_progress")]
    public sealed class GameProgressRow
    {
        // "{userId}|{assignmentId}" so each student keeps their own progress on a shared device.
        [PrimaryKey] public string Key { get; set; }
        public string UserId { get; set; }
        public string AssignmentId { get; set; }
        public string Mode { get; set; }
        public string SessionId { get; set; }
        public string SolvedIndices { get; set; }   // comma-separated board indices
        public int Score { get; set; }
        public int Total { get; set; }
        public int Finished { get; set; }            // 0 / 1
        public long UpdatedAtTicks { get; set; }
    }

    /// <summary>
    /// SQLite-backed <see cref="IGameProgressStore"/> — one row per user + assignment,
    /// serialized through <see cref="SqliteDatabase.Gate"/> like the other stores.
    /// </summary>
    public sealed class SqliteGameProgressStore : IGameProgressStore
    {
        readonly SqliteDatabase db;

        public SqliteGameProgressStore(SqliteDatabase db) => this.db = Guard.NotNull(db, nameof(db));

        static string KeyOf(string userId, string assignmentId)
            => (string.IsNullOrEmpty(userId) ? "anon" : userId) + "|" + (assignmentId ?? "");

        public GameProgress Load(string userId, string assignmentId)
        {
            if (string.IsNullOrEmpty(assignmentId)) return null;
            GameProgressRow row;
            lock (db.Gate) row = db.Connection.Find<GameProgressRow>(KeyOf(userId, assignmentId));
            return row == null ? null : ToModel(row);
        }

        public void Save(GameProgress p)
        {
            if (p == null || string.IsNullOrEmpty(p.AssignmentId)) return;
            var row = ToRow(p);
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        public void Clear(string userId, string assignmentId)
        {
            if (string.IsNullOrEmpty(assignmentId)) return;
            lock (db.Gate) db.Connection.Delete<GameProgressRow>(KeyOf(userId, assignmentId));
        }

        static GameProgressRow ToRow(GameProgress p) => new GameProgressRow
        {
            Key            = KeyOf(p.UserId, p.AssignmentId),
            UserId         = p.UserId,
            AssignmentId   = p.AssignmentId,
            Mode           = p.Mode,
            SessionId      = p.SessionId,
            SolvedIndices  = p.SolvedIndices == null ? "" : string.Join(",", p.SolvedIndices),
            Score          = p.Score,
            Total          = p.Total,
            Finished       = p.Finished ? 1 : 0,
            UpdatedAtTicks = DateTime.UtcNow.Ticks
        };

        static GameProgress ToModel(GameProgressRow r) => new GameProgress
        {
            UserId        = r.UserId,
            AssignmentId  = r.AssignmentId,
            Mode          = r.Mode,
            SessionId     = r.SessionId,
            SolvedIndices = ParseIndices(r.SolvedIndices),
            Score         = r.Score,
            Total         = r.Total,
            Finished      = r.Finished != 0,
            UpdatedUtc    = r.UpdatedAtTicks > 0 ? new DateTime(r.UpdatedAtTicks, DateTimeKind.Utc) : DateTime.MinValue
        };

        static List<int> ParseIndices(string csv)
        {
            var list = new List<int>();
            if (string.IsNullOrEmpty(csv)) return list;
            foreach (var part in csv.Split(','))
                if (int.TryParse(part, out var i)) list.Add(i);
            return list;
        }
    }
}
