using TychoV2.Events;
using TychoV2.Events.Routing;

namespace TychoV2.Persistence
{
    internal class Entry
    {
        public HandlerIdentity HandlerIdentity { get; }

        public IEvent Payload { get; }

        public Entry(HandlerIdentity handlerIdentity, IEvent payload)
        {
            HandlerIdentity = handlerIdentity;
            Payload = payload;
        }
    }
}
