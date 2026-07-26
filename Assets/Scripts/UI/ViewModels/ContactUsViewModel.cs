using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Administration.Services;
using SReader.Domains.Identity.Models;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    public sealed class ContactUsViewModel : ViewModelBase
    {
        readonly ISupportService support;

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";

        public ContactUsViewModel(ISupportService support = null)
        {
            this.support = support;
        }

        public async Task<Result> SendAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName))
                return Fail("Please enter your name.");
            if (!IdentityValidation.IsValidEmail(Email?.Trim()))
                return Fail("Please enter a valid email address.");
            if (string.IsNullOrWhiteSpace(Message))
                return Fail("Please write a message.");
            if (support == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await support.SendMessageAsync(FullName, Email, Subject, Message);
            IsBusy = false;

            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        Result Fail(string message)
        {
            ErrorMessage = message;
            return Result.Fail(message);
        }
    }
}
