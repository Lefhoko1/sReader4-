using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Multiplayer.Models;
using SReader.Domains.Multiplayer.Services;

namespace SReader.Infrastructure.Networking
{
    /// <summary>
    /// INetworkService backed by Unity Netcode for GameObjects.
    /// TODO Phase 8: add the com.unity.netcode.gameobjects package and
    /// drive NetworkManager here. A MirrorNetworkService can be added as
    /// an alternative — game code never knows which one is running.
    /// </summary>
    public sealed class NetcodeNetworkService : INetworkService
    {
        readonly IAppLogger logger;

        public bool IsConnected { get; private set; }

        public NetcodeNetworkService(IAppLogger logger)
        {
            this.logger = Guard.NotNull(logger, nameof(logger));
        }

        public Task<Result> StartHostAsync(MultiplayerSession session, CancellationToken ct = default)
        {
            logger.Warning("StartHostAsync called but networking is not implemented yet (Phase 8).");
            return Task.FromResult(Result.Fail("Multiplayer is not available yet."));
        }

        public Task<Result> JoinAsync(string sessionId, CancellationToken ct = default)
        {
            logger.Warning("JoinAsync called but networking is not implemented yet (Phase 8).");
            return Task.FromResult(Result.Fail("Multiplayer is not available yet."));
        }

        public Task<Result> LeaveAsync(CancellationToken ct = default)
        {
            IsConnected = false;
            return Task.FromResult(Result.Ok());
        }
    }
}
