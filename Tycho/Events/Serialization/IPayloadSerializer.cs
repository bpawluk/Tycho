using System;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Serializes and deserializes event payloads for storage and transport.
    /// </summary>
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Serializes an event payload into a representation suitable for persistence
        /// </summary>
        /// <param name="eventData">The event payload to serialize.</param>
        /// <returns>A serialized representation of <paramref name="eventData"/>.</returns>
        object Serialize(IEvent eventData);

        /// <summary>
        /// Deserializes a previously serialized payload into an event instance of the specified type.
        /// </summary>
        /// <param name="eventType">The target event type.</param>
        /// <param name="payload">The serialized payload.</param>
        /// <returns>An instance of <see cref="IEvent"/> of type <paramref name="eventType"/>.</returns>
        IEvent Deserialize(Type eventType, object payload);

        /// <summary>
        /// Deserializes a previously serialized payload into an event instance of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The target event type.</typeparam>
        /// <param name="payload">The serialized payload.</param>
        /// <returns>An instance of <typeparamref name="TEvent"/>.</returns>
        TEvent Deserialize<TEvent>(object payload) where TEvent : class, IEvent;
    }
}
