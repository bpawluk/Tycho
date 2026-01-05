using System;
using System.Text.Json;
using Tycho.Events;
using Tycho.Events.Serialization;

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
        if (!(payload is string stringPayload) || string.IsNullOrWhiteSpace(stringPayload))
        {
            throw new ArgumentException("Payload must be a non-empty string", nameof(payload));
        }

        var eventData = (IEvent?)JsonSerializer.Deserialize(stringPayload, eventType);
        if (eventData is null)
        {
            throw new InvalidOperationException($"Failed to deserialize payload to {eventType.Name}");
        }

        return eventData;
    }

    public TEvent Deserialize<TEvent>(object payload) where TEvent : class, IEvent
    {
        if (payload is string stringPayload && !string.IsNullOrWhiteSpace(stringPayload))
        {
            var eventData = JsonSerializer.Deserialize<TEvent>(stringPayload);
            if (eventData is null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize payload to {typeof(TEvent).Name}");
            }
            return eventData;
        }
        throw new ArgumentException("Payload must be a non-empty string", nameof(payload));
    }
}