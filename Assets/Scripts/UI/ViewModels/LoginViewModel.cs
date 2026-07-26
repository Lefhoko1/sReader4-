using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    public sealed class LoginViewModel : ViewModelBase
    {
        readonly IAuthenticationService auth;

        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        /// <param name="auth">May be null when no AppCompositionRoot exists yet — sign-in then fails gracefully.</param>
        public LoginViewModel(IAuthenticationService auth = null)
        {
            this.auth = auth;
        }

        public async Task<Result> SignInAsync()
        {
            if (!IdentityValidation.IsValidEmail(Email?.Trim()))
                return Fail("Please enter a valid email address.");
            if (string.IsNullOrEmpty(Password))
                return Fail("Please enter your password.");
            if (auth == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await auth.LoginAsync(Email.Trim(), Password);
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
