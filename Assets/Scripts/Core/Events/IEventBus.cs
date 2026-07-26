using System;

namespace SReader.Core.Events
{
    /// <summary>
    /// Event-driven communication between domains. Publishers and
    /// subscribers never reference each other — e.g. NotificationService
    /// reacts to AssignmentSubmittedEvent without Assignments knowing.
    /// </summary>
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent appEvent) where TEvent : IAppEvent;

        /// <summary>Dispose the returned token to unsubscribe.</summary>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IAppEvent;
    }
}
