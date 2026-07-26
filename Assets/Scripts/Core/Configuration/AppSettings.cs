using UnityEngine;

namespace SReader.Core.Configuration
{
    /// <summary>
    /// Central app configuration asset. Create via
    /// Assets ▸ Create ▸ sReader ▸ App Settings and assign it on the
    /// AppCompositionRoot in the scene.
    ///
    /// SECURITY: only the Supabase anon (public) key belongs here. The
    /// Resend API key must NEVER ship in the client — email is sent by a
    /// Supabase Edge Function that holds the key server-side.
    /// </summary>
    [CreateAssetMenu(fileName = "AppSettings", menuName = "sReader/App Settings")]
    public sealed class AppSettings : ScriptableObject
    {
        public enum AppEnvironment { Development, Staging, Production }

        [Header("Environment")]
        [SerializeField] private AppEnvironment environment = AppEnvironment.Development;

        [Header("Supabase")]
        [SerializeField] private string supabaseUrl = "";
        [SerializeField] private string supabaseAnonKey = "";

        [Header("Photon (Multiplayer)")]
        [Tooltip("Photon PUN App ID (Realtime). Optional — leave blank to use the App ID " +
                 "configured in PhotonServerSettings. Only the public App ID belongs here.")]
        [SerializeField] private string photonAppId = "";

        [Header("Sync")]
        [Tooltip("Seconds between background sync attempts when online.")]
        [SerializeField] private float syncIntervalSeconds = 60f;

        public AppEnvironment Environment => environment;
        public string SupabaseUrl => supabaseUrl;
        public string SupabaseAnonKey => supabaseAnonKey;
        public string PhotonAppId => photonAppId;
        public float SyncIntervalSeconds => syncIntervalSeconds;

        public bool IsSupabaseConfigured =>
            !string.IsNullOrWhiteSpace(supabaseUrl) && !string.IsNullOrWhiteSpace(supabaseAnonKey);
    }
}
