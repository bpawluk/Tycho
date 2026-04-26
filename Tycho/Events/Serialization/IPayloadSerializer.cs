namespace Tycho.Events.Serialization
{
    public interface IPayloadSerializer
    {
        string Serialize<TEvent>(TEvent payload) where TEvent : class, IEvent;

        TEvent Deserialize<TEvent>(string serializedPayload) where TEvent : class, IEvent;
    }
}
