using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using UnityEngine;

namespace SReader.Infrastructure.Supabase
{
    /// <summary>
    /// Thin HTTP helper shared by the Supabase repositories. Awaits resume
    /// on Unity's main thread (UnitySynchronizationContext), so callers can
    /// touch UI after awaiting. Errors come back as failed Results with the
    /// human-readable message Supabase provides.
    /// </summary>
    internal static class SupabaseHttp
    {
        static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        /// <summary>
        /// Optional read-through cache for GETs (set once at the composition
        /// root). Successful GETs are stored; a GET that fails to reach the
        /// network replays the last cached response so data stays visible
        /// offline. Left null in tests / when no cache is wired.
        /// </summary>
        public static ISupabaseReadCache ReadCache { get; set; }

        public static async Task<Result<string>> SendAsync(
            HttpMethod method, string url, string apiKey,
            string jsonBody = null, string bearerToken = null,
            CancellationToken ct = default, string prefer = null)
        {
            var isGet = method == HttpMethod.Get;

            try
            {
                using (var request = new HttpRequestMessage(method, url))
                {
                    request.Headers.Add("apikey", apiKey);
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", bearerToken ?? apiKey);

                    // PostgREST upserts / return-representation need a Prefer header.
                    if (!string.IsNullOrEmpty(prefer))
                        request.Headers.Add("Prefer", prefer);

                    if (jsonBody != null)
                        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using (var response = await client.SendAsync(request, ct))
                    {
                        var body = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            // Cache successful reads so they survive going offline.
                            if (isGet) ReadCache?.Store(url, body ?? "");
                            return Result.Ok(body ?? "");
                        }

                        // A real HTTP status (403/404/…) is an answer, not an
                        // outage — return it rather than masking it with stale cache.
                        return Result.Fail<string>(ExtractError(body, (int)response.StatusCode));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return Result.Fail<string>("The request was cancelled.");
            }
            catch (Exception ex)
            {
                // Couldn't reach the network. For reads, serve the last cached
                // copy if we have one; otherwise surface the connection error.
                if (isGet && ReadCache != null && ReadCache.TryGet(url, out var cached))
                    return Result.Ok(cached ?? "");

                return Result.Fail<string>($"Network error: {ex.Message}");
            }
        }

        [Serializable]
        class ErrorDto
        {
            public string error_description;
            public string msg;
            public string message;
            public string error;
        }

        static string ExtractError(string body, int statusCode)
        {
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var dto = JsonUtility.FromJson<ErrorDto>(body);
                    var text = FirstNonEmpty(dto.error_description, dto.msg, dto.message, dto.error);
                    if (!string.IsNullOrEmpty(text)) return text;
                }
                catch
                {
                    // not JSON — fall through to the generic message
                }
            }
            return $"Request failed ({statusCode}).";
        }

        static string FirstNonEmpty(params string[] values)
        {
            foreach (var v in values)
                if (!string.IsNullOrEmpty(v)) return v;
            return null;
        }
    }
}
