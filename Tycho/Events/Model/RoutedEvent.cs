using System;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Represents an event ready for delivery.
    /// </summary>
    public abstract class RoutedEvent : EventBase
    {
        internal InstanceIdentity DestinationId { get; }

        internal RoutedEvent(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, InstanceIdentity destinationId) : base(id, publishId, eventId, handlerId)
        {
            DestinationId = destinationId;
        }

        internal abstract string SerializePayloadWith(IPayloadSerializer serializer);
    }

    /// <summary>
    /// Represents an event ready for delivery with a strongly typed payload.
    /// </summary>
    /// <typeparam name="TEvent">The event payload type.</typeparam>
    public class RoutedEvent<TEvent> : RoutedEvent where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal RoutedEvent(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, InstanceIdentity destinationEndpointId, TEvent payload) : base(id, publishId, eventId, handlerId, destinationEndpointId)
        {
            Payload = payload;
        }

        internal override string SerializePayloadWith(IPayloadSerializer serializer)
        {
            return serializer.Serialize(Payload);
        }
    }
}
