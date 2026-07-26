using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Configuration;
using SReader.Core.Logging;
using SReader.Domains.Notifications.Services;
using SReader.Infrastructure.Supabase;
using UnityEngine;

namespace SReader.Infrastructure.Resend
{
    /// <summary>
    /// IEmailSender via the 'send-email' Supabase Edge Function
    /// (supabase/functions/send-email), which holds the Resend API key
    /// server-side — the client only ever uses the anon key.
    /// </summary>
    public sealed class ResendEmailSender : IEmailSender
    {
        readonly AppSettings settings;
        readonly IAppLogger logger;

        public ResendEmailSender(AppSettings settings, IAppLogger logger)
        {
            this.settings = Guard.NotNull(settings, nameof(settings));
            this.logger   = Guard.NotNull(logger, nameof(logger));
        }

        public async Task<Result> SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return Result.Fail("Recipient email is required.");
            if (!settings.IsSupabaseConfigured)
                return Result.Fail("Email is unavailable until Supabase is configured.");

            var url = settings.SupabaseUrl.TrimEnd('/') + "/functions/v1/send-email";
            var json = JsonUtility.ToJson(new SendEmailRequest
            {
                to = toEmail,
                subject = subject,
                text = body
            });

            var response = await SupabaseHttp.SendAsync(HttpMethod.Post, url,
                settings.SupabaseAnonKey, json, ct: ct);

            if (response.IsFailure)
            {
                logger.Warning($"send-email failed for '{toEmail}': {response.Error}");
                return Result.Fail(response.Error);
            }
            return Result.Ok();
        }

        [Serializable]
        class SendEmailRequest
        {
            public string to;
            public string subject;
            public string text;
        }
    }
}
