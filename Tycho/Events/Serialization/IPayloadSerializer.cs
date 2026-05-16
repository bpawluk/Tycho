using Tycho.Utils;

namespace Tycho.Events.Serialization
{
    [ReferencedBySourceGenerator]
    public interface IPayloadSerializer
    {
        string Serialize<TEvent>(TEvent payload) where TEvent : class, IEvent;

        TEvent Deserialize<TEvent>(string serializedPayload) where TEvent : class, IEvent;
    }
}
