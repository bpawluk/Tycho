using System;
using System.Collections.Generic;
using Tycho.Events.Routing;
using Tycho.Identity.Events;

namespace Tycho.Events.Serialization
{
    //internal class SomeEventSerializer : EventSerializerBase
    //{
    //    public SomeEventSerializer(IPayloadSerializer payloadSerializer) : base(payloadSerializer)
    //    {
    //        RegisterEvent<SomeEvent>();
    //        RegisterEvent<OtherEvent>();
    //    }
    //}

    internal abstract class EventSerializerBase : IEventSerializer
    {
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly Dictionary<EventIdentity, Func<SerializedEvent, RoutedEvent>> _deserializers;

        protected EventSerializerBase(IPayloadSerializer payloadSerializer)
        {
            _payloadSerializer = payloadSerializer;
            _deserializers = new Dictionary<EventIdentity, Func<SerializedEvent, RoutedEvent>>();
        }

        public SerializedEvent Serialize(RoutedEvent routedEvent)
        {
            return new SerializedEvent(
                routedEvent.Id,
                routedEvent.EventId.ToString(),
                routedEvent.HandlerId.ToString(),
                routedEvent.Route.ToString(),
                routedEvent.SerializePayload(_payloadSerializer));
        }

        public RoutedEvent Deserialize(SerializedEvent serializedEvent)
        {
            var eventId = EventIdentity.Parse(serializedEvent.EventId);
            if (_deserializers.TryGetValue(eventId, out var deserializer))
            {
                return deserializer(serializedEvent);
            }
            throw new InvalidOperationException($"Failed to deserialize an unregistered event with ID {eventId}");
        }

        protected void RegisterEvent<TEvent>() where TEvent : class, IEvent
        {
            var eventId = EventIdentity.Create<TEvent>();
            _deserializers[eventId] = Deserialize<TEvent>;
        }

        private RoutedEvent<TEvent> Deserialize<TEvent>(SerializedEvent serializedEvent) where TEvent : class, IEvent
        {
            var payload = _payloadSerializer.Deserialize<TEvent>(serializedEvent.Payload);
            return RoutedEvent<TEvent>.Create(
                serializedEvent.Id,
                serializedEvent.HandlerId,
                serializedEvent.Route,
                payload);
        }
    }
}
