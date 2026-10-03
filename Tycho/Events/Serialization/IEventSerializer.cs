using Tycho.Events.Model;
using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    /// <summary>
    /// Serializes and deserializes Tycho events.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IEventSerializer
    {
        /// <summary>
        /// Serializes a routed event.
        /// </summary>
        /// <param name="routedEvent">The routed event to serialize.</param>
        /// <returns>The serialized routed event.</returns>
        SerializedRoutedEvent Serialize(RoutedEvent routedEvent);

        /// <summary>
        /// Deserializes a routed event.
        /// </summary>
        /// <param name="serializedEvent">The serialized routed event to deserialize.</param>
        /// <returns>The deserialized routed event.</returns>
        RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent);
    }
}
