# Infrastructure · SQLite

SQLite is **not** a mirror of Supabase. It exists only for:

- Offline access
- Caching
- Downloads
- Pending sync operations

## Planned tables (Phase 7)

| Table | Purpose |
| --- | --- |
| `CachedAssignments` | Offline copies of assignments |
| `CachedLessons` | Offline copies of lessons |
| `CachedProfiles` | Offline copies of user profiles |
| `CachedFriends` | Offline copy of the friends list |
| `CachedImages` | Downloaded image metadata |
| `PendingUploads` | Files awaiting upload |
| `PendingDownloads` | Files awaiting download |
| `PendingSyncOperations` | The sync queue |

## Current state

`SqliteSyncQueueRepository` and `SqliteLocalCache` fulfil the
`ISyncQueueRepository` / `ILocalCache` contracts with in-memory stores so
the rest of the architecture is fully wired today. Phase 7 replaces the
backing store with real SQLite (e.g. `sqlite-net`) **without changing any
interface** — no caller is affected.
