using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Multiplayer.Models;

namespace SReader.Domains.Multiplayer.Services
{
    /// <summary>
    /// Networking port. Game systems depend ONLY on this interface; the
    /// concrete transport (Netcode for GameObjects, Mirror, ...) lives in
    /// Infrastructure.Networking and is swappable.
    /// </summary>
    public interface INetworkService
    {
        bool IsConnected { get; }

        Task<Result> StartHostAsync(MultiplayerSession session, CancellationToken ct = default);
        Task<Result> JoinAsync(string sessionId, CancellationToken ct = default);
        Task<Result> LeaveAsync(CancellationToken ct = default);
    }
}
