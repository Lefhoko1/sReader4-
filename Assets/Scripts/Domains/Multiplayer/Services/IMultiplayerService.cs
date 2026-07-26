using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Multiplayer.Models;

namespace SReader.Domains.Multiplayer.Services
{
    public interface IMultiplayerService
    {
        Task<Result<PlayerProfile>> GetMyProfileAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<MultiplayerSession>>> ListOpenSessionsAsync(CancellationToken ct = default);

        Task<Result> HostSessionAsync(string sessionName, int maxPlayers, CancellationToken ct = default);
        Task<Result> JoinSessionAsync(string sessionId, CancellationToken ct = default);
        Task<Result> LeaveSessionAsync(CancellationToken ct = default);
    }
}
