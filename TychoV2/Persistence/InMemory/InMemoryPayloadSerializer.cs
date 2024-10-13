using System;
using TychoV2.Events;

namespace TychoV2.Persistence.InMemory
{
    internal class InMemoryPayloadSerializer : IPayloadSerializer
    {
        public IEvent Deserialize(Type eventType, object payload) => (payload as IEvent)!;

        public object Serialize(IEvent eventData) => eventData;
    }
}
