using System;
using System.Text.Json;
using Tycho.Events;

namespace Tycho.Persistence.EFCore.Serialization;

internal class PayloadSerializer
{
    private readonly JsonSerializerOptions _jsonOptions = new();

    public object SerializePayload(IEvent eventData)
    {
        if (eventData is null)
        {
            throw new ArgumentNullException(nameof(eventData), "Cannot serialize null event data");
        }

        return JsonSerializer.Serialize(eventData, eventData.GetType(), _jsonOptions);
    }

    public IEvent Deserialize(Type eventType, object payload)
    {
        if (payload is not string stringPayload || string.IsNullOrWhiteSpace(stringPayload))
        {
            throw new ArgumentException("Payload must be a non-empty string", nameof(payload));
        }

        if (JsonSerializer.Deserialize(stringPayload, eventType, _jsonOptions) is not IEvent eventData)
        {
            throw new InvalidOperationException($"Failed to deserialize payload to {eventType.Name}");
        }

        return eventData;
    }

    public TEvent DeserializePayload<TEvent>(object payload) where TEvent : class, IEvent
    {
        if (payload is not string stringPayload || string.IsNullOrWhiteSpace(stringPayload))
        {
            throw new ArgumentException("Payload must be a non-empty string", nameof(payload));
        }

        if (JsonSerializer.Deserialize<TEvent>(stringPayload, _jsonOptions) is not TEvent eventData)
        {
            throw new InvalidOperationException($"Failed to deserialize payload to {typeof(TEvent).Name}");
        }

        return eventData;
    }
}