using System;
using System.Collections.Generic;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    [ReferencedBySourceGenerator]
    public abstract class EventSerializerBase : IEventSerializer
    {
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly Dictionary<EventIdentity, Func<RoutedEvent, SerializedRoutedEvent>> _serializers;
        private readonly Dictionary<EventIdentity, Func<SerializedRoutedEvent, RoutedEvent>> _deserializers;

        [ReferencedBySourceGenerator]
        protected EventSerializerBase(IPayloadSerializer payloadSerializer)
        {
            _payloadSerializer = payloadSerializer;
            _serializers = new Dictionary<EventIdentity, Func<RoutedEvent, SerializedRoutedEvent>>();
            _deserializers = new Dictionary<EventIdentity, Func<SerializedRoutedEvent, RoutedEvent>>();
        }

        public SerializedRoutedEvent Serialize(RoutedEvent routedEvent)
        {
            if (_serializers.TryGetValue(routedEvent.EventId, out var serializer))
            {
                return serializer(routedEvent);
            }
            throw new InvalidOperationException($"Failed to serialize an unregistered event with ID {routedEvent.EventId}");
        }

        public RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent)
        {
            if (_deserializers.TryGetValue(serializedEvent.EventId, out var deserializer))
            {
                return deserializer(serializedEvent);
            }
            throw new InvalidOperationException($"Failed to deserialize an unregistered event with ID {serializedEvent.EventId}");
        }

        [ReferencedBySourceGenerator]
        protected void RegisterEvent<TEvent>() where TEvent : class, IEvent
        {
            var eventId = EventIdentity.Create<TEvent>();
            _serializers[eventId] = routedEvent =>
            {
                if (routedEvent is RoutedEvent<TEvent> typedRoutedEvent)
                {
                    return Serialize(typedRoutedEvent);
                }
                throw new InvalidOperationException($"Failed to serialize event with ID {routedEvent.EventId} because it is not of the expected type {typeof(TEvent).Name}");
            };
            _deserializers[eventId] = Deserialize<TEvent>;
        }

        private SerializedRoutedEvent Serialize<TEvent>(RoutedEvent<TEvent> routedEvent) where TEvent : class, IEvent
        {
            var payload = _payloadSerializer.Serialize(routedEvent.Payload);
            return new SerializedRoutedEvent(
                routedEvent.Id,
                routedEvent.EventId,
                routedEvent.HandlerId,
                routedEvent.Route,
                payload);
        }

        private RoutedEvent<TEvent> Deserialize<TEvent>(SerializedRoutedEvent serializedEvent) where TEvent : class, IEvent
        {
            var payload = _payloadSerializer.Deserialize<TEvent>(serializedEvent.Payload);
            return new RoutedEvent<TEvent>(
                serializedEvent.Id,
                serializedEvent.EventId,
                serializedEvent.HandlerId,
                serializedEvent.Route,
                payload);
        }
    }
}
