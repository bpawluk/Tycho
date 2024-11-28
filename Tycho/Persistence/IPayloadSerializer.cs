using System;
using Tycho.Events;

namespace Tycho.Persistence
{
    internal interface IPayloadSerializer
    {
        object Serialize(IEvent eventData);

        IEvent Deserialize(Type eventType, object payload);
    }
}