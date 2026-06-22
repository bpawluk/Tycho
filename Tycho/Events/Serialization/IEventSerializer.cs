using Tycho.Events.Model;
using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Serializes and deserializes Tycho Events.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IEventSerializer
    {
        /// <summary>
        /// Serializes a routed Event.
        /// </summary>
        /// <param name="routedEvent">The routed Event to serialize.</param>
        /// <returns>The serialized routed Event.</returns>
        SerializedRoutedEvent Serialize(RoutedEvent routedEvent);

        /// <summary>
        /// Deserializes a routed Event.
        /// </summary>
        /// <param name="serializedEvent">The serialized routed Event to deserialize.</param>
        /// <returns>The deserialized routed Event.</returns>
        RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent);
    }
}
