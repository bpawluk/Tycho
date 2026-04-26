namespace Tycho.Events.Serialization
{
    public interface IPayloadSerializer
    {
        object Serialize<TEvent>(TEvent payload) where TEvent : class, IEvent;

        TEvent Deserialize<TEvent>(object serializedPayload) where TEvent : class, IEvent;
    }
}
