namespace Tycho.Events.Serialization
{
    internal class InMemoryPayloadSerializer : IPayloadSerializer
    {
        public object Serialize<TEvent>(TEvent payload) 
            where TEvent : class, IEvent 
            => payload;

        public TEvent Deserialize<TEvent>(object serializedPayload) 
            where TEvent : class, IEvent 
            => (serializedPayload as TEvent)!;
    }
}
