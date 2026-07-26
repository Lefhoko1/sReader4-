using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;

namespace SReader.Domains.Notifications.Services
{
    /// <summary>
    /// Outbound email port — implemented by Infrastructure.Resend (via a
    /// Supabase Edge Function so the Resend API key never ships in the app).
    /// </summary>
    public interface IEmailSender
    {
        Task<Result> SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
    }
}
