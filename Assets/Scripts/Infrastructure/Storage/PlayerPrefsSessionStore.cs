using System;
using System.Threading.Tasks;
using SReader.Core.Authentication;
using UnityEngine;

namespace SReader.Infrastructure.Storage
{
    /// <summary>
    /// Session persistence for session restore. PlayerPrefs is fine for a
    /// first pass; swap for encrypted storage / Android Keystore later —
    /// callers only know ISessionStore.
    /// </summary>
    public sealed class PlayerPrefsSessionStore : ISessionStore
    {
        const string Key = "sreader.auth.session";

        [Serializable]
        class SessionDto
        {
            public string userId;
            public string email;
            public string accessToken;
            public string refreshToken;
            public long expiresAtUtcTicks;
        }

        public Task SaveAsync(AuthSession session)
        {
            if (session == null) return ClearAsync();

            var dto = new SessionDto
            {
                userId = session.UserId,
                email = session.Email,
                accessToken = session.AccessToken,
                refreshToken = session.RefreshToken,
                expiresAtUtcTicks = session.ExpiresAtUtc.Ticks
            };
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(dto));
            PlayerPrefs.Save();
            return Task.CompletedTask;
        }

        public Task<AuthSession> LoadAsync()
        {
            var json = PlayerPrefs.GetString(Key, null);
            if (string.IsNullOrEmpty(json))
                return Task.FromResult<AuthSession>(null);

            try
            {
                var dto = JsonUtility.FromJson<SessionDto>(json);
                return Task.FromResult(new AuthSession
                {
                    UserId = dto.userId,
                    Email = dto.email,
                    AccessToken = dto.accessToken,
                    RefreshToken = dto.refreshToken,
                    ExpiresAtUtc = new DateTime(dto.expiresAtUtcTicks, DateTimeKind.Utc)
                });
            }
            catch
            {
                // A corrupt entry must not lock the user out of the app.
                PlayerPrefs.DeleteKey(Key);
                return Task.FromResult<AuthSession>(null);
            }
        }

        public Task ClearAsync()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
            return Task.CompletedTask;
        }
    }
}
