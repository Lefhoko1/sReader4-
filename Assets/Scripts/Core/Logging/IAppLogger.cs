namespace SReader.Core.Logging
{
    /// <summary>
    /// Logging abstraction — domain and application code never calls
    /// UnityEngine.Debug directly, so layers below the UI stay free of
    /// Unity dependencies and logs can be redirected (file, remote).
    /// </summary>
    public interface IAppLogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
}
