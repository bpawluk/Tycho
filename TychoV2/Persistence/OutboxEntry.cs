using TychoV2.Events.Routing;

namespace TychoV2.Persistence
{
    internal class OutboxEntry
    {
        public HandlerIdentity HandlerIdentity { get; }

        public object Payload { get; }

        public OutboxEntry(HandlerIdentity handlerIdentity, object payload)
        {
            HandlerIdentity = handlerIdentity;
            Payload = payload;
        }
    }
}
