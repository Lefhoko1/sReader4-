# SQLite setup — SQLitePCLRaw (e_sqlite3)

We use the single-file **sqlite-net** wrapper (`SQLite.cs`, namespace `SQLite`) on
top of **SQLitePCLRaw** as the native engine. SQLitePCLRaw bundles its own tested
SQLite for every platform (Android `.so` per ABI, iOS static lib, Windows/macOS
editor), so it does NOT depend on whatever the device provides. This is the engine
Microsoft.Data.Sqlite / EF Core / MAUI use — proven on Android and iOS.

> Why we left the old approach: hand-sourcing a `libsqliteX.so` / `sqlite3.dll`
> "worked" in the editor but hard-crashed on a Huawei Y5 (EMUI) — a loose native
> lib can collide with / mis-depend on the system. SQLitePCLRaw carries its own,
> so that class of crash goes away.

## One-time install

1. **Install NuGetForUnity** (free): Window ▸ Package Manager ▸ ➕ ▸ *Add package from
   git URL* → `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity`
2. **Add the package**: NuGet ▸ Manage NuGet Packages ▸ search **`SQLitePCLRaw.bundle_e_sqlite3`**
   ▸ Install. (This pulls `SQLitePCLRaw.core`, `.provider.e_sqlite3`, `.batteries_v2`
   and the native `lib.e_sqlite3` for all platforms.)
3. **Add the scripting define**: Project Settings ▸ Player ▸ *Scripting Define Symbols*
   → add `USE_SQLITEPCL_RAW` (for every platform tab). This switches `SQLite.cs` to the
   SQLitePCLRaw path, which auto-calls `Batteries_V2.Init()`.
4. **Anti-stripping**: `Assets/link.xml` is already in the project — keep it. It stops
   IL2CPP from stripping the provider on Android/iOS release builds.

## Remove the old hand-sourced binaries (after the above builds clean)

These are no longer used once `USE_SQLITEPCL_RAW` is set — delete to avoid confusion:
- `Assets/Plugins/Android/libs/**/libsqliteX.so` (+ metas)
- `Assets/Plugins/x86_64/sqliteX.dll`
- `sqliteX.dll` in the project root

## Verify

- **Editor**: press Play → log shows `[SQLite] Database ready at …/sreader.db`.
- **Android device**: rebuild, install, and make sure offline still works — turn on the
  app online once (so data caches), then enable Airplane mode: pages/academies should
  still load and the amber "Offline — showing saved data" banner appears.

The `enableOfflineStorage` toggle on the **App (Composition Root)** GameObject must be
**ticked** for offline to run. (Untick it only to force online-only.)

## Code (no changes needed)

`SQLiteDatabase`, `SqliteLocalCache`, `SqliteSyncQueueRepository`,
`SqliteSupabaseReadCache` and `OfflineFirstEducationRepository` all use the `SQLite`
namespace API and are unaffected by which native engine backs it. The composition root
opens the DB in a try/catch and falls back to online-only if SQLite can't initialise.
