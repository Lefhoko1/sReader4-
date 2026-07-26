using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Files.Services;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// Uploads images to any Supabase Storage bucket and returns their public
    /// URL. Objects are stored under the user's id with a random suffix so
    /// updates don't collide and paths are unguessable.
    /// </summary>
    public sealed class SupabaseImageUploader : IImageUploader
    {
        static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

        readonly AppSettings settings;
        readonly CurrentSessionHolder session;

        public SupabaseImageUploader(AppSettings settings, CurrentSessionHolder session)
        {
            this.settings = Guard.NotNull(settings, nameof(settings));
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<string>> UploadAsync(string bucket, byte[] data, string extension, string keyPrefix, CancellationToken ct = default)
        {
            if (!settings.IsSupabaseConfigured)
                return Result.Fail<string>("Supabase is not configured.");
            if (data == null || data.Length == 0)
                return Result.Fail<string>("No image to upload.");

            var baseUrl = settings.SupabaseUrl.TrimEnd('/');
            var ext = string.IsNullOrWhiteSpace(extension) ? "png" : extension.ToLowerInvariant();
            var contentType = (ext == "jpg" || ext == "jpeg") ? "image/jpeg" : "image/png";

            var userId = session?.CurrentUserId ?? "anonymous";
            var safePrefix = string.IsNullOrWhiteSpace(keyPrefix) ? "img" : keyPrefix;
            var objectPath = $"{userId}/{safePrefix}-{Guid.NewGuid():N}.{ext}";
            var uploadUrl = $"{baseUrl}/storage/v1/object/{bucket}/{objectPath}";
            var token = session?.Session?.AccessToken ?? settings.SupabaseAnonKey;

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl))
                {
                    request.Headers.Add("apikey", settings.SupabaseAnonKey);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    request.Headers.Add("x-upsert", "true");

                    var content = new ByteArrayContent(data);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    request.Content = content;

                    using (var response = await client.SendAsync(request, ct))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var body = await response.Content.ReadAsStringAsync();
                            return Result.Fail<string>($"Upload failed ({(int)response.StatusCode}): {body}");
                        }
                    }
                }

                return Result.Ok($"{baseUrl}/storage/v1/object/public/{bucket}/{objectPath}");
            }
            catch (OperationCanceledException)
            {
                return Result.Fail<string>("The upload was cancelled.");
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Upload error: {ex.Message}");
            }
        }
    }
}
