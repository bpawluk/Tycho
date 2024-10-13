using System;
using TychoV2.Events;

namespace TychoV2.Persistence
{
    internal interface IPayloadSerializer
    {
        object Serialize(IEvent eventData);

        IEvent Deserialize(Type eventType, object payload);
    }
}
