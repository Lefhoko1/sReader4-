using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Social.Models;
using SReader.Domains.Social.Repositories;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// A local mirror row for one friendship. <see cref="PendingOp"/> records an
    /// unsynced local change: "" (synced), "create", "update" or "delete".
    /// Offline-created rows get a temporary "local_…" id until the server
    /// assigns the real uuid on the next flush.
    /// </summary>
    [Table("friendships")]
    public sealed class FriendshipRow
    {
        [PrimaryKey] public string Id { get; set; }
        public string RequesterId { get; set; }
        public string RecipientId { get; set; }
        public string Status { get; set; }
        public long CreatedAtTicks { get; set; }
        public string PendingOp { get; set; }

        public const string Synced = "";
        public const string Create = "create";
        public const string Update = "update";
        public const string Delete = "delete";

        public static FriendshipRow From(Friendship f, string pendingOp) => new FriendshipRow
        {
            Id             = f.Id,
            RequesterId    = f.RequesterId,
            RecipientId    = f.RecipientId,
            Status         = f.Status.ToString(),
            CreatedAtTicks = f.CreatedAt.Ticks,
            PendingOp      = pendingOp ?? Synced
        };

        public Friendship ToModel() => new Friendship
        {
            Id          = Id,
            RequesterId = RequesterId,
            RecipientId = RecipientId,
            Status      = Enum.TryParse<FriendshipStatus>(Status, true, out var s) ? s : FriendshipStatus.Pending,
            CreatedAt   = new DateTime(CreatedAtTicks, DateTimeKind.Utc)
        };
    }

    /// <summary>
    /// Offline-first decorator over the Supabase friendship repository, for the
    /// friendship rows themselves. Reads serve the live server when online
    /// (refreshing the local mirror) and the SQLite mirror when offline. Writes
    /// (send / accept / decline / remove) go straight to Supabase when online;
    /// offline they are recorded in the mirror with a PendingOp and pushed on the
    /// next online read via <see cref="FlushPendingAsync"/>. The people directory
    /// and friend-settings reads pass through — they're already cached for offline
    /// use at the HTTP layer (SupabaseHttp.ReadCache).
    /// </summary>
    public sealed class OfflineFirstFriendshipRepository : IFriendshipRepository
    {
        readonly IFriendshipRepository inner;
        readonly SqliteDatabase db;
        readonly IConnectivity connectivity;
        readonly IAppLogger logger;

        bool flushing;

        public OfflineFirstFriendshipRepository(IFriendshipRepository inner, SqliteDatabase db,
            IConnectivity connectivity, IAppLogger logger)
        {
            this.inner        = Guard.NotNull(inner, nameof(inner));
            this.db           = Guard.NotNull(db, nameof(db));
            this.connectivity = Guard.NotNull(connectivity, nameof(connectivity));
            this.logger       = Guard.NotNull(logger, nameof(logger));
        }

        // ── Reads (offline-first) ────────────────────────────────────────────

        public async Task<Result<IReadOnlyList<Friendship>>> ListForUserAsync(string userId, FriendshipStatus status, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListForUserAsync(userId, status, ct);
                if (remote.IsSuccess) { RefreshMirror(remote.Value); return remote; }
                logger.Warning($"[Offline] List friendships failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalFor(userId, status));
        }

        public async Task<Result<IReadOnlyList<Friendship>>> ListAllForUserAsync(string userId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                await FlushPendingAsync(ct);
                var remote = await inner.ListAllForUserAsync(userId, ct);
                if (remote.IsSuccess) { RefreshMirror(remote.Value); return remote; }
                logger.Warning($"[Offline] List all friendships failed online, serving local mirror: {remote.Error}");
            }
            return Result.Ok(LocalFor(userId, null));
        }

        public async Task<Result<Friendship>> GetAsync(string friendshipId, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var remote = await inner.GetAsync(friendshipId, ct);
                if (remote.IsSuccess) { Upsert(FriendshipRow.From(remote.Value, FriendshipRow.Synced)); return remote; }
                logger.Warning($"[Offline] Get friendship failed online, serving local mirror: {remote.Error}");
            }
            var row = Find(friendshipId);
            return row == null || row.PendingOp == FriendshipRow.Delete
                ? Result.Fail<Friendship>("Friendship not found.")
                : Result.Ok(row.ToModel());
        }

        public async Task<Result<Friendship>> FindBetweenAsync(string userA, string userB, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var remote = await inner.FindBetweenAsync(userA, userB, ct);
                if (remote.IsSuccess)
                {
                    if (remote.Value != null) Upsert(FriendshipRow.From(remote.Value, FriendshipRow.Synced));
                    return remote;
                }
                logger.Warning($"[Offline] Find friendship failed online, serving local mirror: {remote.Error}");
            }

            var match = AllLocal().FirstOrDefault(r =>
                r.PendingOp != FriendshipRow.Delete &&
                ((r.RequesterId == userA && r.RecipientId == userB) ||
                 (r.RequesterId == userB && r.RecipientId == userA)));
            return Result.Ok(match?.ToModel());
        }

        // ── Writes (offline-queued) ──────────────────────────────────────────

        public async Task<Result> CreateAsync(Friendship friendship, CancellationToken ct = default)
        {
            if (connectivity.IsOnline)
            {
                var created = await inner.CreateAsync(friendship, ct);
                if (created.IsSuccess)
                {
                    // The server generates the id; we'll pull the real row on the next list.
                    return created;
                }
                logger.Warning($"[Offline] Create friendship failed online, saving locally: {created.Error}");
            }

            if (string.IsNullOrEmpty(friendship.Id))
                friendship.Id = "local_" + Guid.NewGuid().ToString("N");
            Upsert(FriendshipRow.From(friendship, FriendshipRow.Create));
            return Result.Ok();
        }

        public async Task<Result> UpdateStatusAsync(string friendshipId, FriendshipStatus status, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(friendshipId)) return Result.Fail("Friendship id is required.");

            if (connectivity.IsOnline)
            {
                var updated = await inner.UpdateStatusAsync(friendshipId, status, ct);
                if (updated.IsSuccess)
                {
                    var r = Find(friendshipId);
                    if (r != null) { r.Status = status.ToString(); r.PendingOp = FriendshipRow.Synced; Upsert(r); }
                    return updated;
                }
                logger.Warning($"[Offline] Update friendship failed online, saving locally: {updated.Error}");
            }

            var row = Find(friendshipId);
            if (row == null) return Result.Fail("Friendship not found offline.");
            row.Status = status.ToString();
            // A row still pending creation stays "create"; otherwise it's an update.
            if (row.PendingOp != FriendshipRow.Create) row.PendingOp = FriendshipRow.Update;
            Upsert(row);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(string friendshipId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(friendshipId)) return Result.Fail("Friendship id is required.");

            if (connectivity.IsOnline)
            {
                var deleted = await inner.DeleteAsync(friendshipId, ct);
                if (deleted.IsSuccess) { Remove(friendshipId); return deleted; }
                logger.Warning($"[Offline] Delete friendship failed online, saving locally: {deleted.Error}");
            }

            var row = Find(friendshipId);
            if (row == null) return Result.Ok();
            if (row.PendingOp == FriendshipRow.Create) Remove(friendshipId); // never reached the server
            else { row.PendingOp = FriendshipRow.Delete; Upsert(row); }
            return Result.Ok();
        }

        /// <summary>
        /// Push every queued friendship change to Supabase. Best-effort: a failed
        /// op is left pending and retried on the next flush. Re-entrant-safe.
        /// </summary>
        public async Task FlushPendingAsync(CancellationToken ct = default)
        {
            if (flushing || !connectivity.IsOnline) return;
            flushing = true;
            try
            {
                List<FriendshipRow> pending;
                lock (db.Gate)
                {
                    pending = db.Connection.Table<FriendshipRow>()
                        .Where(r => r.PendingOp != FriendshipRow.Synced)
                        .ToList();
                }

                foreach (var row in pending)
                {
                    if (ct.IsCancellationRequested) break;

                    switch (row.PendingOp)
                    {
                        case FriendshipRow.Create:
                        {
                            var created = await inner.CreateAsync(row.ToModel(), ct);
                            if (created.IsSuccess) Remove(row.Id); // drop temp id; real row arrives on next list
                            else logger.Warning($"[Offline] Flush create friendship deferred: {created.Error}");
                            break;
                        }
                        case FriendshipRow.Update:
                        {
                            var status = Enum.TryParse<FriendshipStatus>(row.Status, true, out var s) ? s : FriendshipStatus.Pending;
                            var updated = await inner.UpdateStatusAsync(row.Id, status, ct);
                            if (updated.IsSuccess) { row.PendingOp = FriendshipRow.Synced; Upsert(row); }
                            else logger.Warning($"[Offline] Flush update friendship deferred: {updated.Error}");
                            break;
                        }
                        case FriendshipRow.Delete:
                        {
                            var deleted = await inner.DeleteAsync(row.Id, ct);
                            if (deleted.IsSuccess) Remove(row.Id);
                            else logger.Warning($"[Offline] Flush delete friendship deferred: {deleted.Error}");
                            break;
                        }
                    }
                }
            }
            finally
            {
                flushing = false;
            }
        }

        // ── Pass-through (directory + settings: cached at the HTTP layer) ─────

        public Task<Result<FriendSettings>> GetFriendSettingsAsync(string userId, CancellationToken ct = default)
            => inner.GetFriendSettingsAsync(userId, ct);

        public Task<Result> UpsertFriendSettingsAsync(FriendSettings settings, CancellationToken ct = default)
            => inner.UpsertFriendSettingsAsync(settings, ct);

        public Task<Result<IReadOnlyList<PersonSummary>>> ListPeopleAsync(CancellationToken ct = default)
            => inner.ListPeopleAsync(ct);

        public Task<Result<PersonSummary>> GetPersonAsync(string userId, CancellationToken ct = default)
            => inner.GetPersonAsync(userId, ct);

        public Task<Result<IReadOnlyList<StudentSubject>>> ListStudentAcademicsAsync(string studentId, CancellationToken ct = default)
            => inner.ListStudentAcademicsAsync(studentId, ct);

        // ── SQLite mirror helpers ────────────────────────────────────────────

        FriendshipRow Find(string id)
        {
            lock (db.Gate) return db.Connection.Find<FriendshipRow>(id);
        }

        void Upsert(FriendshipRow row)
        {
            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        void Remove(string id)
        {
            lock (db.Gate) db.Connection.Delete<FriendshipRow>(id);
        }

        List<FriendshipRow> AllLocal()
        {
            lock (db.Gate) return db.Connection.Table<FriendshipRow>().ToList();
        }

        IReadOnlyList<Friendship> LocalFor(string userId, FriendshipStatus? status)
        {
            var statusText = status?.ToString();
            return AllLocal()
                .Where(r => r.PendingOp != FriendshipRow.Delete)
                .Where(r => r.RequesterId == userId || r.RecipientId == userId)
                .Where(r => statusText == null || r.Status == statusText)
                .OrderByDescending(r => r.CreatedAtTicks)
                .Select(r => r.ToModel())
                .ToList();
        }

        /// <summary>
        /// Replace mirror rows with a fresh server list, but never clobber a row
        /// that still has unsynced local changes (those win until flushed).
        /// </summary>
        void RefreshMirror(IReadOnlyList<Friendship> remote)
        {
            lock (db.Gate)
            {
                foreach (var f in remote)
                {
                    var existing = db.Connection.Find<FriendshipRow>(f.Id);
                    if (existing != null && existing.PendingOp != FriendshipRow.Synced) continue;
                    db.Connection.InsertOrReplace(FriendshipRow.From(f, FriendshipRow.Synced));
                }
            }
        }
    }
}
