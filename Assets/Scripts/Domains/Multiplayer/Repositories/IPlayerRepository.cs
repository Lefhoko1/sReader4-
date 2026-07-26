using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Multiplayer.Models;

namespace SReader.Domains.Multiplayer.Repositories
{
    /// <summary>Persistence only — live multiplayer state is never stored in Supabase.</summary>
    public interface IPlayerRepository
    {
        Task<Result<PlayerProfile>> GetProfileAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertProfileAsync(PlayerProfile profile, CancellationToken ct = default);

        Task<Result<PlayerState>> GetStateAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertStateAsync(PlayerState state, CancellationToken ct = default);

        Task<Result<IReadOnlyList<MultiplayerSession>>> ListOpenSessionsAsync(CancellationToken ct = default);
        Task<Result> SaveSessionAsync(MultiplayerSession session, CancellationToken ct = default);
        Task<Result> CloseSessionAsync(string sessionId, CancellationToken ct = default);
    }
}
