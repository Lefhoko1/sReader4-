namespace SReader.Domains.Identity.Services
{
    /// <summary>
    /// Carries state across the ForgotPassword → OTP → ResetPassword
    /// screens, so each page's ViewModel stays independent and no page
    /// passes data through the navigation layer.
    /// </summary>
    public sealed class PasswordResetFlow
    {
        public string Email { get; private set; }
        public string VerifiedCode { get; private set; }

        public bool HasEmail => !string.IsNullOrEmpty(Email);
        public bool IsCodeVerified => !string.IsNullOrEmpty(VerifiedCode);

        public void Begin(string email)
        {
            Email = email;
            VerifiedCode = null;
        }

        public void MarkCodeVerified(string code) => VerifiedCode = code;

        public void Complete()
        {
            Email = null;
            VerifiedCode = null;
        }
    }
}
