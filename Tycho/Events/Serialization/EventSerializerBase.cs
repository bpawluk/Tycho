using System;
using System.Collections.Generic;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Base class for generated event serializers.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class EventSerializerBase : IEventSerializer
    {
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly Dictionary<EventIdentity, EventRegistration> _deserializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSerializerBase"/> class.
        /// </summary>
        /// <param name="payloadSerializer">The underlying event payload serializer.</param>
        [ReferencedBySourceGenerator]
        protected EventSerializerBase(IPayloadSerializer payloadSerializer)
        {
            _payloadSerializer = payloadSerializer;
            _deserializers = new Dictionary<EventIdentity, EventRegistration>();
        }

        /// <inheritdoc/>
        public SerializedRoutedEvent Serialize(RoutedEvent routedEvent)
        {
            string serializedPayload = routedEvent.SerializePayloadWith(_payloadSerializer);
            return new SerializedRoutedEvent(
                routedEvent.Id,
                routedEvent.PublishId,
                routedEvent.EventId,
                routedEvent.HandlerId,
                routedEvent.DestinationId,
                serializedPayload);
        }

        /// <inheritdoc/>
        public Event Deserialize(SerializedEvent serializedEvent)
        {
            if (_deserializers.TryGetValue(serializedEvent.EventId, out EventRegistration registration))
            {
                return registration.Deserialize(serializedEvent);
            }
            throw new InvalidOperationException($"Failed to deserialize an unregistered event with ID {serializedEvent.EventId}");
        }

        /// <summary>
        /// Registers an event payload type for generated deserialization.
        /// </summary>
        /// <typeparam name="TEvent">The event payload type.</typeparam>
        [ReferencedBySourceGenerator]
        protected void RegisterEvent<TEvent>() where TEvent : class, IEvent
        {
            var eventId = EventIdentity.Create<TEvent>();
            if (_deserializers.TryGetValue(eventId, out EventRegistration registration))
            {
                if (registration.EventType != typeof(TEvent))
                {
                    throw new InvalidOperationException(
                        $"Event ID '{eventId}' is shared by '{registration.EventType}' and '{typeof(TEvent)}'.");
                }

                return;
            }

            _deserializers.Add(eventId, new EventRegistration(typeof(TEvent), Deserialize<TEvent>));
        }

        private Event<TEvent> Deserialize<TEvent>(SerializedEvent serializedEvent) where TEvent : class, IEvent
        {
            TEvent payload = _payloadSerializer.Deserialize<TEvent>(serializedEvent.Payload);
            return new Event<TEvent>(
                serializedEvent.Id,
                serializedEvent.PublishId,
                serializedEvent.EventId,
                serializedEvent.HandlerId,
                payload);
        }

        private readonly struct EventRegistration
        {
            public Type EventType { get; }

            public Func<SerializedEvent, Event> Deserialize { get; }

            public EventRegistration(Type eventType, Func<SerializedEvent, Event> deserialize)
            {
                EventType = eventType;
                Deserialize = deserialize;
            }
        }
    }
}
