using System;
using System.Security.Cryptography;
using System.Text;
using SQLite;
using SReader.Core.Authentication;
using SReader.Core.Common;

namespace SReader.Infrastructure.SQLite
{
    /// <summary>
    /// One saved offline-login record. The password is NEVER stored in plain
    /// text — only a PBKDF2 hash + per-user random salt, so the on-disk file
    /// can't be read back into a password. The last <see cref="AuthSession"/>
    /// is kept (as JSON) so an offline sign-in can hand back real tokens for the
    /// read-cache / token-refresh-on-next-launch to work.
    /// </summary>
    [Table("offline_credentials")]
    public sealed class OfflineCredentialRow
    {
        // Normalised (trimmed, lower-cased) email — one credential per account.
        [PrimaryKey] public string Email { get; set; }
        public string UserId { get; set; }
        public string PasswordHash { get; set; }   // base64
        public string PasswordSalt { get; set; }   // base64
        public int Iterations { get; set; }
        public string SessionJson { get; set; }     // serialized AuthSession
        public long UpdatedAtTicks { get; set; }
    }

    /// <summary>
    /// Stores and verifies login credentials on the device so the user can sign
    /// in without a connection. A credential is written every time the user logs
    /// in (or registers) successfully online; afterwards the same email+password
    /// can be verified locally while offline.
    /// </summary>
    public sealed class OfflineCredentialStore
    {
        const int Iterations = 100_000;
        const int SaltBytes  = 16;
        const int HashBytes  = 32;

        readonly SqliteDatabase db;

        public OfflineCredentialStore(SqliteDatabase db)
        {
            this.db = Guard.NotNull(db, nameof(db));
        }

        static string Normalize(string email) => (email ?? "").Trim().ToLowerInvariant();

        /// <summary>Hashes the password and saves (or replaces) the credential for this account.</summary>
        public void Save(string email, string password, AuthSession session)
        {
            email = Normalize(email);
            if (email.Length == 0 || string.IsNullOrEmpty(password)) return;

            var salt = new byte[SaltBytes];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var row = new OfflineCredentialRow
            {
                Email          = email,
                UserId         = session?.UserId,
                PasswordSalt   = Convert.ToBase64String(salt),
                PasswordHash   = Convert.ToBase64String(Hash(password, salt, Iterations)),
                Iterations     = Iterations,
                SessionJson    = SessionCodec.Serialize(session),
                UpdatedAtTicks = DateTime.UtcNow.Ticks
            };

            lock (db.Gate) db.Connection.InsertOrReplace(row);
        }

        /// <summary>
        /// Returns the stored session when <paramref name="password"/> matches the
        /// saved credential for <paramref name="email"/>; otherwise null (no record
        /// or wrong password).
        /// </summary>
        public AuthSession Verify(string email, string password)
        {
            email = Normalize(email);
            if (email.Length == 0 || string.IsNullOrEmpty(password)) return null;

            OfflineCredentialRow row;
            lock (db.Gate) row = db.Connection.Find<OfflineCredentialRow>(email);
            if (row == null || string.IsNullOrEmpty(row.PasswordHash) || string.IsNullOrEmpty(row.PasswordSalt))
                return null;

            byte[] salt, expected;
            try
            {
                salt     = Convert.FromBase64String(row.PasswordSalt);
                expected = Convert.FromBase64String(row.PasswordHash);
            }
            catch { return null; }

            var actual = Hash(password, salt, row.Iterations > 0 ? row.Iterations : Iterations);
            if (!FixedTimeEquals(actual, expected)) return null;

            return SessionCodec.Deserialize(row.SessionJson);
        }

        /// <summary>True if an offline credential exists for this account.</summary>
        public bool Has(string email)
        {
            email = Normalize(email);
            if (email.Length == 0) return false;
            lock (db.Gate) return db.Connection.Find<OfflineCredentialRow>(email) != null;
        }

        static byte[] Hash(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                return pbkdf2.GetBytes(HashBytes);
        }

        // Constant-time compare so a wrong password can't be timed byte-by-byte.
        static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        /// <summary>Tiny AuthSession ⇄ JSON codec (kept here so the store is self-contained).</summary>
        static class SessionCodec
        {
            [Serializable]
            class Dto
            {
                public string userId;
                public string email;
                public string accessToken;
                public string refreshToken;
                public long expiresAtUtcTicks;
            }

            public static string Serialize(AuthSession s)
            {
                if (s == null) return "";
                return UnityEngine.JsonUtility.ToJson(new Dto
                {
                    userId            = s.UserId,
                    email             = s.Email,
                    accessToken       = s.AccessToken,
                    refreshToken      = s.RefreshToken,
                    expiresAtUtcTicks = s.ExpiresAtUtc.Ticks
                });
            }

            public static AuthSession Deserialize(string json)
            {
                if (string.IsNullOrEmpty(json)) return null;
                try
                {
                    var dto = UnityEngine.JsonUtility.FromJson<Dto>(json);
                    if (dto == null) return null;
                    return new AuthSession
                    {
                        UserId       = dto.userId,
                        Email        = dto.email,
                        AccessToken  = dto.accessToken,
                        RefreshToken = dto.refreshToken,
                        ExpiresAtUtc = new DateTime(dto.expiresAtUtcTicks, DateTimeKind.Utc)
                    };
                }
                catch { return null; }
            }
        }
    }
}
