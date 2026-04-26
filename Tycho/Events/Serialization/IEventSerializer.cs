using Tycho.Events.Model;

namespace Tycho.Events.Serialization
{
    public interface IEventSerializer
    {
        SerializedRoutedEvent Serialize(RoutedEvent routedEvent);

        RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent);
    }
}
