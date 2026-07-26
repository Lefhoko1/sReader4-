using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Education.Services
{
    /// <summary>
    /// Uploads a proof-of-payment image to storage and returns a URL the tutor
    /// can view. Implemented over Supabase Storage in Infrastructure.
    /// </summary>
    public interface IPaymentProofUploader
    {
        Task<Result<string>> UploadAsync(byte[] data, string extension, string requestId, CancellationToken ct = default);
    }
}
