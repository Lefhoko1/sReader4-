using SReader.Core.Common;
using UnityEngine;

namespace SReader.Infrastructure.Networking
{
    /// <summary>
    /// Reachability via Unity's Application.internetReachability. This reports
    /// whether a network route exists (Wi-Fi / carrier), not whether Supabase is
    /// actually reachable — a failed request still falls back to the local cache.
    /// </summary>
    public sealed class UnityConnectivity : IConnectivity
    {
        /// <summary>Editor/testing override: when true the app behaves as if offline.</summary>
        public bool ForceOffline { get; set; }

        public bool IsOnline => !ForceOffline && Application.internetReachability != NetworkReachability.NotReachable;
    }
}
