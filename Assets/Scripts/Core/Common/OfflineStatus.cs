namespace SReader.Core.Common
{
    /// <summary>
    /// Runtime status of the on-device offline storage (SQLite). Set once by the
    /// composition root and read by the UI so the user (and we, while debugging)
    /// can see whether data is actually being cached locally on this device.
    /// </summary>
    public sealed class OfflineStatus
    {
        /// <summary>True when the SQLite cache/queue initialised and offline data is live.</summary>
        public bool StorageReady { get; set; }

        /// <summary>Short human-readable reason/detail (e.g. "ready", "disabled", an error).</summary>
        public string Detail { get; set; } = "initialising";
    }
}
