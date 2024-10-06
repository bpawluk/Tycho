using TychoV2.Events;

namespace TychoV2.Persistence
{
    public class Entry
    {
        public string HandlerId { get; }

        public IEvent Payload { get; }

        public Entry(string handlerId, IEvent payload)
        {
            HandlerId = handlerId;
            Payload = payload;
        }
    }
}
