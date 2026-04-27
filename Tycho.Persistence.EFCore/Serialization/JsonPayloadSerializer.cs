using System;
using System.Text.Json;
using Tycho.Events;
using Tycho.Events.Serialization;

namespace Tycho.Persistence.EFCore.Serialization;

internal class JsonPayloadSerializer : IPayloadSerializer
{
    private readonly JsonSerializerOptions _jsonOptions = new();

    public object Serialize<TEvent>(TEvent eventData) where TEvent : class, IEvent
    {
        if (eventData is null)
        {
            throw new ArgumentNullException(nameof(eventData), "Cannot serialize null event data");
        }

        return JsonSerializer.Serialize(eventData, eventData.GetType(), _jsonOptions);
    }

    public TEvent Deserialize<TEvent>(object payload) where TEvent : class, IEvent
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