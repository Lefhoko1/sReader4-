using UnityEngine;

namespace SReader.Core.Logging
{
    /// <summary>Default IAppLogger that forwards to the Unity console.</summary>
    public sealed class UnityAppLogger : IAppLogger
    {
        readonly string prefix;

        public UnityAppLogger(string prefix = "sReader")
        {
            this.prefix = prefix;
        }

        public void Info(string message)    => Debug.Log($"[{prefix}] {message}");
        public void Warning(string message) => Debug.LogWarning($"[{prefix}] {message}");
        public void Error(string message)   => Debug.LogError($"[{prefix}] {message}");
    }
}
