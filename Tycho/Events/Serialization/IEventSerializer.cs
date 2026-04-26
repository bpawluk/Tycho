using Tycho.Events.Routing;

namespace Tycho.Events.Serialization
{
    public interface IEventSerializer
    {
        SerializedEvent Serialize(RoutedEvent routedEvent);

        RoutedEvent Deserialize(SerializedEvent serializedEvent);
    }
}
