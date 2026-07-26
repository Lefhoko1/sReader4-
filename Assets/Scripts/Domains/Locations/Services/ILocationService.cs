using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Locations.Models;

namespace SReader.Domains.Locations.Services
{
    public interface ILocationService
    {
        Task<Result<UserLocation>> GetMyLocationAsync(CancellationToken ct = default);

        /// <summary>Returns another user's location only if their privacy settings allow it.</summary>
        Task<Result<UserLocation>> GetVisibleLocationAsync(string ownerUserId, CancellationToken ct = default);

        Task<Result> UpdateMyLocationAsync(UserLocation location, CancellationToken ct = default);
        Task<Result> ClearMyLocationAsync(CancellationToken ct = default);
    }
}
