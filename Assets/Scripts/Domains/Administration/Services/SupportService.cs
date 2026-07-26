using System.Threading;
using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Core.Logging;
using SReader.Domains.Administration.Models;
using SReader.Domains.Administration.Repositories;
using SReader.Domains.Identity.Models;
using SReader.Domains.Notifications.Services;

namespace SReader.Domains.Administration.Services
{
    public sealed class SupportService : ISupportService
    {
        readonly ISupportRepository repository;
        readonly IEmailSender emailSender;
        readonly IAppLogger logger;
        readonly IClock clock;

        public SupportService(ISupportRepository repository, IEmailSender emailSender, IAppLogger logger, IClock clock)
        {
            this.repository  = Guard.NotNull(repository, nameof(repository));
            this.emailSender = Guard.NotNull(emailSender, nameof(emailSender));
            this.logger      = Guard.NotNull(logger, nameof(logger));
            this.clock       = Guard.NotNull(clock, nameof(clock));
        }

        public async Task<Result> SendMessageAsync(string fullName, string email, string subject, string message, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return Result.Fail("Please enter your name.");
            if (!IdentityValidation.IsValidEmail(email?.Trim()))
                return Result.Fail("Please enter a valid email address.");
            if (string.IsNullOrWhiteSpace(message))
                return Result.Fail("Please write a message.");

            var support = new SupportMessage
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                Subject = string.IsNullOrWhiteSpace(subject) ? "(no subject)" : subject.Trim(),
                Message = message.Trim(),
                SentAt = clock.UtcNow
            };

            var saved = await repository.SaveMessageAsync(support, ct);
            if (saved.IsFailure) return saved;

            // Confirmation email is best-effort; the stored message is what counts.
            var emailed = await emailSender.SendAsync(support.Email, "We received your message",
                "Thanks for contacting sReader — we'll be in touch within one school day.", ct);
            if (emailed.IsFailure)
                logger.Warning($"Support message stored but confirmation email failed: {emailed.Error}");

            return Result.Ok();
        }
    }
}
