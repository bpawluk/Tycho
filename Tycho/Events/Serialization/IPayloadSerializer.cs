using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Serializes and deserializes Tycho Event payload values.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Serializes an Event payload.
        /// </summary>
        /// <typeparam name="TEvent">The Event payload type.</typeparam>
        /// <param name="payload">The Event payload to serialize.</param>
        /// <returns>The serialized payload.</returns>
        string Serialize<TEvent>(TEvent payload) where TEvent : class, IEvent;

        /// <summary>
        /// Deserializes an Event payload.
        /// </summary>
        /// <typeparam name="TEvent">The Event payload type.</typeparam>
        /// <param name="serializedPayload">The serialized payload.</param>
        /// <returns>The deserialized Event payload.</returns>
        TEvent Deserialize<TEvent>(string serializedPayload) where TEvent : class, IEvent;
    }
}
