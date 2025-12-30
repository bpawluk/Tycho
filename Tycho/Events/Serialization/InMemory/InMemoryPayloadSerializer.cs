using System;

namespace Tycho.Events.Serialization.InMemory
{
    internal class InMemoryPayloadSerializer : IPayloadSerializer
    {
        public object Serialize(IEvent eventData)
        {
            return eventData;
        }

        public IEvent Deserialize(Type eventType, object payload)
        {
            return (payload as IEvent)!;
        }

        public TEvent Deserialize<TEvent>(object payload)
            where TEvent : class, IEvent
        {
            return (payload as TEvent)!;
        }
    }
}
