namespace SReader.Core.Common
{
    /// <summary>
    /// Whether the device currently has a usable network path. Abstracted so the
    /// offline-first repositories are testable without the real radio, and so the
    /// "online?" decision lives in one place.
    /// </summary>
    public interface IConnectivity
    {
        bool IsOnline { get; }
    }
}
