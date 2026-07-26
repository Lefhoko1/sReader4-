using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Multiplayer.Models;
using SReader.Domains.Multiplayer.Repositories;

namespace SReader.Domains.Multiplayer.Services
{
    /// <summary>
    /// Coordinates session persistence (repository) with the live
    /// transport (INetworkService) — the two concerns stay separate.
    /// </summary>
    public sealed class MultiplayerService : IMultiplayerService
    {
        readonly IPlayerRepository repository;
        readonly INetworkService network;
        readonly CurrentSessionHolder sessionHolder;
        readonly IAppLogger logger;
        readonly IClock clock;

        public MultiplayerService(IPlayerRepository repository, INetworkService network,
                                  CurrentSessionHolder sessionHolder, IAppLogger logger, IClock clock)
        {
            this.repository    = Guard.NotNull(repository, nameof(repository));
            this.network       = Guard.NotNull(network, nameof(network));
            this.sessionHolder = Guard.NotNull(sessionHolder, nameof(sessionHolder));
            this.logger        = Guard.NotNull(logger, nameof(logger));
            this.clock         = Guard.NotNull(clock, nameof(clock));
        }

        public Task<Result<PlayerProfile>> GetMyProfileAsync(CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn)
                return Task.FromResult(Result.Fail<PlayerProfile>("Not signed in."));
            return repository.GetProfileAsync(sessionHolder.CurrentUserId, ct);
        }

        public Task<Result<IReadOnlyList<MultiplayerSession>>> ListOpenSessionsAsync(CancellationToken ct = default)
            => repository.ListOpenSessionsAsync(ct);

        public async Task<Result> HostSessionAsync(string sessionName, int maxPlayers, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Result.Fail("Not signed in.");
            if (string.IsNullOrWhiteSpace(sessionName)) return Result.Fail("Session name is required.");
            if (maxPlayers < 1) return Result.Fail("Max players must be at least 1.");

            var session = new MultiplayerSession
            {
                HostUserId = sessionHolder.CurrentUserId,
                SessionName = sessionName.Trim(),
                MaxPlayers = maxPlayers,
                CreatedAt = clock.UtcNow
            };

            var started = await network.StartHostAsync(session, ct);
            if (started.IsFailure) return started;

            var saved = await repository.SaveSessionAsync(session, ct);
            if (saved.IsFailure)
                logger.Warning($"Session started but could not be persisted: {saved.Error}");
            return Result.Ok();
        }

        public Task<Result> JoinSessionAsync(string sessionId, CancellationToken ct = default)
        {
            if (!sessionHolder.IsSignedIn) return Task.FromResult(Result.Fail("Not signed in."));
            if (string.IsNullOrEmpty(sessionId)) return Task.FromResult(Result.Fail("Session id is required."));
            return network.JoinAsync(sessionId, ct);
        }

        public Task<Result> LeaveSessionAsync(CancellationToken ct = default)
            => network.LeaveAsync(ct);
    }
}
