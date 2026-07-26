using SReader.Core.Common;

namespace SReader.Domains.Identity.Models
{
    /// <summary>
    /// Credential rules shared by ViewModels (instant feedback) and
    /// AuthenticationService (authoritative check) — the single source of
    /// truth, so the UI hint "8+ characters with at least one number"
    /// always matches what the service enforces.
    /// </summary>
    public static class IdentityValidation
    {
        public const int MinPasswordLength = 8;
        public const int OtpLength = 6;

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            int at = email.IndexOf('@');
            return at > 0 && at < email.Length - 1;
        }

        public static Result ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
                return Result.Fail($"Password must be at least {MinPasswordLength} characters.");

            bool hasDigit = false;
            foreach (var c in password)
                if (char.IsDigit(c)) { hasDigit = true; break; }

            return hasDigit ? Result.Ok() : Result.Fail("Password must contain at least one number.");
        }
    }
}
