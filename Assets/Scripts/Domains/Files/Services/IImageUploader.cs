using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Files.Services
{
    /// <summary>
    /// Uploads an image to a storage bucket and returns a viewable URL.
    /// Bucket-parameterised so it serves avatars, proofs, etc.
    /// </summary>
    public interface IImageUploader
    {
        Task<Result<string>> UploadAsync(string bucket, byte[] data, string extension, string keyPrefix, CancellationToken ct = default);
    }
}
