namespace SReader.Core.Authentication
{
    /// <summary>
    /// In-memory holder for the active session. AuthenticationService
    /// writes it on login/logout; other services read it to know the
    /// current user. Null Session means "signed out".
    /// </summary>
    public sealed class CurrentSessionHolder
    {
        public AuthSession Session { get; private set; }

        public bool IsSignedIn => Session != null;
        public string CurrentUserId => Session?.UserId;

        public void Set(AuthSession session) => Session = session;
        public void Clear() => Session = null;
    }
}
