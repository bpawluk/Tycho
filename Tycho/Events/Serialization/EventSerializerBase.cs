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
        private readonly Dictionary<EventIdentity, Func<SerializedRoutedEvent, RoutedEvent>> _deserializers;

        [ReferencedBySourceGenerator]
        protected EventSerializerBase(IPayloadSerializer payloadSerializer)
        {
            _payloadSerializer = payloadSerializer;
            _deserializers = new Dictionary<EventIdentity, Func<SerializedRoutedEvent, RoutedEvent>>();
        }

        public SerializedRoutedEvent Serialize(RoutedEvent routedEvent)
        {
            string serializedPayload = routedEvent.SerializePayloadWith(_payloadSerializer);
            return new SerializedRoutedEvent(
                routedEvent.Id,
                routedEvent.PublishId,
                routedEvent.EventId,
                routedEvent.HandlerId,
                routedEvent.Route,
                serializedPayload);
        }

        public RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent)
        {
            if (_deserializers.TryGetValue(serializedEvent.EventId, out Func<SerializedRoutedEvent, RoutedEvent>? deserializer))
            {
                return deserializer(serializedEvent);
            }
            throw new InvalidOperationException($"Failed to deserialize an unregistered event with ID {serializedEvent.EventId}");
        }

        [ReferencedBySourceGenerator]
        protected void RegisterEvent<TEvent>() where TEvent : class, IEvent
        {
            var eventId = EventIdentity.Create<TEvent>();
            _deserializers[eventId] = Deserialize<TEvent>;
        }

        private RoutedEvent<TEvent> Deserialize<TEvent>(SerializedRoutedEvent serializedEvent) where TEvent : class, IEvent
        {
            TEvent payload = _payloadSerializer.Deserialize<TEvent>(serializedEvent.Payload);
            return new RoutedEvent<TEvent>(
                serializedEvent.Id,
                serializedEvent.PublishId,
                serializedEvent.EventId,
                serializedEvent.HandlerId,
                serializedEvent.Route,
                payload);
        }
    }
}
