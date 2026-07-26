using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Multiplayer.Models;
using SReader.Domains.Multiplayer.Repositories;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// TODO Phase 8: player_profiles / player_states / multiplayer_sessions tables.
    /// Persistence only — live multiplayer state never touches Supabase.
    /// </summary>
    public sealed class SupabasePlayerRepository : SupabaseRepositoryBase, IPlayerRepository
    {
        public SupabasePlayerRepository(AppSettings settings) : base(settings) { }

        public Task<Result<PlayerProfile>> GetProfileAsync(string userId, CancellationToken ct = default)
            => TodoAsync<PlayerProfile>("Load player profile");

        public Task<Result> UpsertProfileAsync(PlayerProfile profile, CancellationToken ct = default)
            => TodoAsync("Save player profile");

        public Task<Result<PlayerState>> GetStateAsync(string userId, CancellationToken ct = default)
            => TodoAsync<PlayerState>("Load player state");

        public Task<Result> UpsertStateAsync(PlayerState state, CancellationToken ct = default)
            => TodoAsync("Save player state");

        public Task<Result<IReadOnlyList<MultiplayerSession>>> ListOpenSessionsAsync(CancellationToken ct = default)
            => TodoAsync<IReadOnlyList<MultiplayerSession>>("List sessions");

        public Task<Result> SaveSessionAsync(MultiplayerSession session, CancellationToken ct = default)
            => TodoAsync("Save session");

        public Task<Result> CloseSessionAsync(string sessionId, CancellationToken ct = default)
            => TodoAsync("Close session");
    }
}
