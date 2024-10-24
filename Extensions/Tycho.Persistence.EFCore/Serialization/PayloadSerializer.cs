using System;
using System.Text.Json;
using Tycho.Events;

namespace Tycho.Persistence.EFCore.Serialization;

internal class PayloadSerializer : IPayloadSerializer
{
    public object Serialize(IEvent eventData)
    {
        if (eventData is null)
        {
            throw new ArgumentNullException(nameof(eventData), "Cannot serialize null event data");
        }
        return JsonSerializer.Serialize(eventData, eventData.GetType());
    }

    public IEvent Deserialize(Type eventType, object payload)
    {
        if (payload is string stringPayload)
        {
            var eventData = JsonSerializer.Deserialize(stringPayload, eventType) as IEvent;
            if (eventData is null)
            {
                throw new InvalidOperationException($"Failed to deserialize payload to {eventType.Name}");
            }
            return eventData;
        }
        throw new ArgumentException("Payload must be a non-null string", nameof(payload));
    }
}
