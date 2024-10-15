using System;
using Tycho.Events;

namespace Tycho.Persistence.InMemory
{
    internal class InMemoryPayloadSerializer : IPayloadSerializer
    {
        public IEvent Deserialize(Type eventType, object payload)
        {
            return (payload as IEvent)!;
        }

        public object Serialize(IEvent eventData)
        {
            return eventData;
        }
    }
}