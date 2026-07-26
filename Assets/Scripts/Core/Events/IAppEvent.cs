namespace SReader.Core.Events
{
    /// <summary>
    /// Marker for events published on the IEventBus. Each domain defines
    /// its own event types (e.g. UserLoggedInEvent in Identity).
    /// </summary>
    public interface IAppEvent
    {
    }
}
