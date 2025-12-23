using System;

namespace Tycho.Events.Serialization
{
    internal interface IPayloadSerializer
    {
        object Serialize(IEvent eventData);

        IEvent Deserialize(Type eventType, object payload);
    }
}
