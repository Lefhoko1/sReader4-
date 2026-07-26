using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Locations.Models;

namespace SReader.Domains.Locations.Repositories
{
    public interface IUserLocationRepository
    {
        Task<Result<UserLocation>> GetForUserAsync(string userId, CancellationToken ct = default);
        Task<Result> UpsertAsync(UserLocation location, CancellationToken ct = default);
        Task<Result> DeleteForUserAsync(string userId, CancellationToken ct = default);
    }
}
