using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    public sealed class ResetPasswordViewModel : ViewModelBase
    {
        readonly IAuthenticationService auth;

        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";

        public ResetPasswordViewModel(IAuthenticationService auth = null)
        {
            this.auth = auth;
        }

        public async Task<Result> UpdatePasswordAsync()
        {
            var policy = IdentityValidation.ValidatePassword(NewPassword);
            if (policy.IsFailure)
            {
                ErrorMessage = policy.Error;
                return policy;
            }
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Result.Fail(ErrorMessage);
            }
            if (auth == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await auth.CompletePasswordResetAsync(NewPassword);
            IsBusy = false;

            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }
    }
}
