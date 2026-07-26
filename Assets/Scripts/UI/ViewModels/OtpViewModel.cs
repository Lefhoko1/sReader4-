using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    public sealed class OtpViewModel : ViewModelBase
    {
        readonly IAuthenticationService auth;

        public OtpViewModel(IAuthenticationService auth = null)
        {
            this.auth = auth;
        }

        public async Task<Result> VerifyAsync(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length < IdentityValidation.OtpLength)
            {
                ErrorMessage = $"Please enter all {IdentityValidation.OtpLength} digits.";
                return Result.Fail(ErrorMessage);
            }
            if (auth == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await auth.VerifyResetCodeAsync(code);
            IsBusy = false;

            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        public async Task<Result> ResendAsync(string email = null)
        {
            ClearError();
            if (auth == null)
                return PlaceholderOk();

            // Re-uses the email captured by the PasswordResetFlow inside the service.
            IsBusy = true;
            var result = await auth.RequestPasswordResetAsync(email ?? "");
            IsBusy = false;
            return result;
        }
    }
}
