using System;
using System.Text.Json;

namespace Tycho.Events.Serialization
{
    internal class JsonPayloadSerializer : IPayloadSerializer
    {
        private readonly JsonPayloadSerializerSettings _settings;

        public JsonPayloadSerializer(JsonPayloadSerializerSettings settings)
        {
            _settings = settings;
        }

        public string Serialize<TEvent>(TEvent eventData) where TEvent : class, IEvent
        {
            if (eventData != null)
            {
                return JsonSerializer.Serialize(eventData, eventData.GetType(), _settings.JsonOptions);
            }
            throw new ArgumentNullException(nameof(eventData), "Cannot serialize null event data");
        }

        public TEvent Deserialize<TEvent>(string payload) where TEvent : class, IEvent
        {
            if (!string.IsNullOrWhiteSpace(payload))
            {
                if (JsonSerializer.Deserialize<TEvent>(payload, _settings.JsonOptions) is TEvent eventData)
                {
                    return eventData;
                }
                throw new InvalidOperationException($"Failed to deserialize payload to {typeof(TEvent).Name}");
            }
            throw new ArgumentException("Payload must be a non-empty string", nameof(payload));
        }
    }
}
