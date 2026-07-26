using System.Threading.Tasks;
using SReader.Core.Common;
using SReader.Domains.Identity.Models;
using SReader.Domains.Identity.Services;
using SReader.UI.Bindings;

namespace SReader.UI.ViewModels
{
    /// <summary>
    /// Backs the three-step register form. Step validation lives here —
    /// not in the MonoBehaviour — per the "no business logic in views" rule.
    /// </summary>
    public sealed class RegisterViewModel : ViewModelBase
    {
        readonly IAuthenticationService auth;

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
        public bool AcceptedTerms { get; set; }

        /// <summary>Which kind of account to create — drives what the user can do in the app.</summary>
        public UserRole Role { get; set; } = UserRole.Student;

        public RegisterViewModel(IAuthenticationService auth = null)
        {
            this.auth = auth;
        }

        /// <summary>Step 0 — names + email.</summary>
        public Result ValidateIdentityStep()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                return Fail("Please enter your first and last name.");
            if (!IdentityValidation.IsValidEmail(Email?.Trim()))
                return Fail("Please enter a valid email address.");
            return Pass();
        }

        /// <summary>Step 1 — password + confirmation.</summary>
        public Result ValidateSecurityStep()
        {
            var policy = IdentityValidation.ValidatePassword(Password);
            if (policy.IsFailure) return Fail(policy.Error);
            if (Password != ConfirmPassword)
                return Fail("Passwords do not match.");
            return Pass();
        }

        public async Task<Result> RegisterAsync()
        {
            var identity = ValidateIdentityStep();
            if (identity.IsFailure) return identity;

            var security = ValidateSecurityStep();
            if (security.IsFailure) return security;

            if (!AcceptedTerms)
                return Fail("Please agree to the Terms of Service.");
            if (auth == null)
                return PlaceholderOk();

            IsBusy = true;
            var result = await auth.RegisterAsync(Email.Trim(), Password, FirstName.Trim(), LastName.Trim(), Role);
            IsBusy = false;

            ErrorMessage = result.IsFailure ? result.Error : "";
            return result;
        }

        Result Fail(string message)
        {
            ErrorMessage = message;
            return Result.Fail(message);
        }

        Result Pass()
        {
            ErrorMessage = "";
            return Result.Ok();
        }
    }
}
