using System.Threading.Tasks;

namespace SReader.Core.Authentication
{
    /// <summary>
    /// Persists the auth session across app launches (session restore).
    /// Implemented in Infrastructure (PlayerPrefsSessionStore).
    /// </summary>
    public interface ISessionStore
    {
        Task SaveAsync(AuthSession session);
        Task<AuthSession> LoadAsync();
        Task ClearAsync();
    }
}
