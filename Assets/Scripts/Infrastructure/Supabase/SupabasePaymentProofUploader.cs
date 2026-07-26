using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Domains.Education.Services;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// Uploads proof-of-payment images to the Supabase Storage bucket
    /// "payment-proofs" and returns their public URL. Objects are stored under
    /// the student's user id so storage policies can scope them.
    /// </summary>
    public sealed class SupabasePaymentProofUploader : IPaymentProofUploader
    {
        const string Bucket = "payment-proofs";
        static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };

        readonly AppSettings settings;
        readonly CurrentSessionHolder session;

        public SupabasePaymentProofUploader(AppSettings settings, CurrentSessionHolder session)
        {
            this.settings = Guard.NotNull(settings, nameof(settings));
            this.session = Guard.NotNull(session, nameof(session));
        }

        public async Task<Result<string>> UploadAsync(byte[] data, string extension, string requestId, CancellationToken ct = default)
        {
            if (!settings.IsSupabaseConfigured)
                return Result.Fail<string>("Supabase is not configured.");
            if (data == null || data.Length == 0)
                return Result.Fail<string>("No image to upload.");

            var baseUrl = settings.SupabaseUrl.TrimEnd('/');
            var ext = string.IsNullOrWhiteSpace(extension) ? "png" : extension.ToLowerInvariant();
            var contentType = (ext == "jpg" || ext == "jpeg") ? "image/jpeg" : "image/png";

            var userId = session?.CurrentUserId ?? "anonymous";
            var objectPath = $"{userId}/{requestId}-{Guid.NewGuid():N}.{ext}";
            var uploadUrl = $"{baseUrl}/storage/v1/object/{Bucket}/{objectPath}";
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

                // Public URL (the bucket is public-read; paths are unguessable).
                var publicUrl = $"{baseUrl}/storage/v1/object/public/{Bucket}/{objectPath}";
                return Result.Ok(publicUrl);
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
