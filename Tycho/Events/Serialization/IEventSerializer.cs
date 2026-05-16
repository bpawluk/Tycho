using Tycho.Events.Model;
using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    [ReferencedBySourceGenerator]
    public interface IEventSerializer
    {
        SerializedRoutedEvent Serialize(RoutedEvent routedEvent);

        RoutedEvent Deserialize(SerializedRoutedEvent serializedEvent);
    }
}
