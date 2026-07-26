using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Administration.Models;

namespace SReader.Domains.Administration.Repositories
{
    public interface ISupportRepository
    {
        Task<Result> SaveMessageAsync(SupportMessage message, CancellationToken ct = default);
    }
}
