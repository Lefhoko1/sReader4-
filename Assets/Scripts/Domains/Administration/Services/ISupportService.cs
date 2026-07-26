using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Administration.Services
{
    public interface ISupportService
    {
        Task<Result> SendMessageAsync(string fullName, string email, string subject, string message, CancellationToken ct = default);
    }
}
