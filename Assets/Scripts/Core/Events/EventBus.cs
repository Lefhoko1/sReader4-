using System;
using System.Collections.Generic;

namespace SReader.Core.Events
{
    /// <summary>
    /// Simple synchronous in-process event bus. A handler that throws is
    /// isolated so it cannot break the other subscribers of the event.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        readonly Dictionary<Type, List<Delegate>> handlers = new Dictionary<Type, List<Delegate>>();
        readonly object gate = new object();

        public void Publish<TEvent>(TEvent appEvent) where TEvent : IAppEvent
        {
            Delegate[] snapshot;
            lock (gate)
            {
                if (!handlers.TryGetValue(typeof(TEvent), out var list) || list.Count == 0)
                    return;
                snapshot = list.ToArray();
            }

            foreach (var handler in snapshot)
            {
                try
                {
                    ((Action<TEvent>)handler)(appEvent);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"[EventBus] Handler for {typeof(TEvent).Name} threw: {ex}");
                }
            }
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IAppEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (gate)
            {
                if (!handlers.TryGetValue(typeof(TEvent), out var list))
                {
                    list = new List<Delegate>();
                    handlers[typeof(TEvent)] = list;
                }
                list.Add(handler);
            }

            return new Subscription(this, typeof(TEvent), handler);
        }

        void Unsubscribe(Type eventType, Delegate handler)
        {
            lock (gate)
            {
                if (handlers.TryGetValue(eventType, out var list))
                    list.Remove(handler);
            }
        }

        sealed class Subscription : IDisposable
        {
            EventBus bus;
            readonly Type eventType;
            readonly Delegate handler;

            public Subscription(EventBus bus, Type eventType, Delegate handler)
            {
                this.bus = bus;
                this.eventType = eventType;
                this.handler = handler;
            }

            public void Dispose()
            {
                bus?.Unsubscribe(eventType, handler);
                bus = null;
            }
        }
    }
}
