using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Files.Services;

namespace SReader.Infrastructure.Storage
{
    /// <summary>
    /// IFileStorage over Application.persistentDataPath — downloaded
    /// lessons, assignments and images live here for offline use.
    /// </summary>
    public sealed class LocalFileStorage : IFileStorage
    {
        readonly string rootPath;

        /// <param name="rootPath">
        /// Pass Application.persistentDataPath from the composition root —
        /// Unity APIs may only be touched on the main thread, so the path
        /// is captured once up front.
        /// </param>
        public LocalFileStorage(string rootPath)
        {
            this.rootPath = Guard.NotNullOrEmpty(rootPath, nameof(rootPath));
        }

        public async Task<Result> SaveAsync(string relativePath, byte[] data, CancellationToken ct = default)
        {
            if (data == null) return Result.Fail("No data to save.");
            try
            {
                var fullPath = FullPath(relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await stream.WriteAsync(data, 0, data.Length, ct);
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Could not save '{relativePath}': {ex.Message}");
            }
        }

        public async Task<Result<byte[]>> LoadAsync(string relativePath, CancellationToken ct = default)
        {
            try
            {
                var fullPath = FullPath(relativePath);
                if (!File.Exists(fullPath))
                    return Result.Fail<byte[]>($"File '{relativePath}' does not exist.");

                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    var buffer = new byte[stream.Length];
                    int read = 0;
                    while (read < buffer.Length)
                    {
                        int n = await stream.ReadAsync(buffer, read, buffer.Length - read, ct);
                        if (n == 0) break;
                        read += n;
                    }
                    return Result.Ok(buffer);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<byte[]>($"Could not load '{relativePath}': {ex.Message}");
            }
        }

        public Task<bool> ExistsAsync(string relativePath)
            => Task.FromResult(File.Exists(FullPath(relativePath)));

        public Task<Result> DeleteAsync(string relativePath)
        {
            try
            {
                var fullPath = FullPath(relativePath);
                if (File.Exists(fullPath)) File.Delete(fullPath);
                return Task.FromResult(Result.Ok());
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Fail($"Could not delete '{relativePath}': {ex.Message}"));
            }
        }

        string FullPath(string relativePath)
        {
            Guard.NotNullOrEmpty(relativePath, nameof(relativePath));
            return Path.Combine(rootPath, relativePath);
        }
    }
}
