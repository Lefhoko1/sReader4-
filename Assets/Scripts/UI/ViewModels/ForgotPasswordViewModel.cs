using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    public sealed class ForgotPasswordViewModel : ViewModelBase
    {
        readonly IAuthenticationService auth;

        public string Email { get; set; } = "";

        public ForgotPasswordViewModel(IAuthenticationService auth = null)
        {
            this.auth = auth;
        }

        public async Task<Result> SendCodeAsync()
        {
            if (!IdentityValidation.IsValidEmail(Email?.Trim()))
            {
                ErrorMessage = "Please enter a valid email address.";
                return Result.Fail(ErrorMessage);
            }
            if (auth == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await auth.RequestPasswordResetAsync(Email.Trim());
            IsBusy = false;

            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }
    }
}
