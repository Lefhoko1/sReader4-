using UnityEngine;

namespace SReader.Core.Configuration
{
    /// <summary>Which networking stack drives multiplayer.</summary>
    public enum MultiplayerBackend
    {
        /// <summary>Supabase Realtime (Phoenix WebSocket) over the DB-backed session.</summary>
        Supabase,
        /// <summary>Photon PUN 2 — network-native rooms/state, no database writes.</summary>
        Photon
    }

    /// <summary>
    /// The user's chosen multiplayer backend, persisted on the device. This is an
    /// explicit CHOICE (surfaced by an in-app selector), not an automatic fallback —
    /// the user picks Photon or Supabase and that pick is honoured. If Photon isn't
    /// compiled in (PUN not imported) it can't be selected and the value is forced to
    /// Supabase.
    /// </summary>
    public sealed class MultiplayerBackendSetting
    {
        const string Key = "sreader.mp.backend";

        /// <summary>True when Photon PUN is compiled in (PHOTON_UNITY_NETWORKING).</summary>
        public bool PhotonAvailable { get; }

        public MultiplayerBackend Backend { get; private set; }

        public MultiplayerBackendSetting(bool photonAvailable)
        {
            PhotonAvailable = photonAvailable;

            var saved = PlayerPrefs.GetString(Key, "");
            if (saved == "photon" && photonAvailable) Backend = MultiplayerBackend.Photon;
            else if (saved == "supabase")             Backend = MultiplayerBackend.Supabase;
            else Backend = photonAvailable ? MultiplayerBackend.Photon : MultiplayerBackend.Supabase; // default to Photon when present
        }

        public void Set(MultiplayerBackend backend)
        {
            if (backend == MultiplayerBackend.Photon && !PhotonAvailable)
                backend = MultiplayerBackend.Supabase;
            Backend = backend;
            PlayerPrefs.SetString(Key, backend == MultiplayerBackend.Photon ? "photon" : "supabase");
            PlayerPrefs.Save();
        }

        public bool UsingPhoton => Backend == MultiplayerBackend.Photon;
        public string Label => Backend == MultiplayerBackend.Photon ? "Photon" : "Supabase";
    }
}
