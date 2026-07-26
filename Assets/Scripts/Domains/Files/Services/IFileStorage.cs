using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Files.Services
{
    /// <summary>
    /// Local file persistence port — implemented by Infrastructure.Storage
    /// against Application.persistentDataPath.
    /// </summary>
    public interface IFileStorage
    {
        Task<Result> SaveAsync(string relativePath, byte[] data, CancellationToken ct = default);
        Task<Result<byte[]>> LoadAsync(string relativePath, CancellationToken ct = default);
        Task<bool> ExistsAsync(string relativePath);
        Task<Result> DeleteAsync(string relativePath);
    }
}
