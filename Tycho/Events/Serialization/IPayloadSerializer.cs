using System;

namespace Tycho.Events.Serialization
{
    public interface IPayloadSerializer
    {
        object Serialize(IEvent eventData);

        IEvent Deserialize(Type eventType, object payload);

        TEvent Deserialize<TEvent>(object payload) where TEvent : class, IEvent;
    }
}
