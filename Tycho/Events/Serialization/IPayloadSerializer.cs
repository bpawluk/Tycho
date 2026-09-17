using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Serializes and deserializes Tycho event payload values.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Serializes an event payload.
        /// </summary>
        /// <typeparam name="TEvent">The event payload type.</typeparam>
        /// <param name="payload">The Event payload to serialize.</param>
        /// <returns>The serialized payload.</returns>
        string Serialize<TEvent>(TEvent payload) where TEvent : class, IEvent;

        /// <summary>
        /// Deserializes an event payload.
        /// </summary>
        /// <typeparam name="TEvent">The event payload type.</typeparam>
        /// <param name="serializedPayload">The serialized payload.</param>
        /// <returns>The deserialized event payload.</returns>
        TEvent Deserialize<TEvent>(string serializedPayload) where TEvent : class, IEvent;
    }
}
