using System;

namespace SReader.Core.Authentication
{
    /// <summary>
    /// The authenticated session of the current user — held in Core so
    /// every layer can know "who is logged in" without referencing the
    /// Identity domain or Supabase.
    /// </summary>
    public sealed class AuthSession
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }

        public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAtUtc;
    }
}
