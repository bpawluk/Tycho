using System;

namespace Tycho.Events.Serialization
{
    public class SerializedEvent
    {
        public Guid Id { get; }

        public string EventId { get; }

        public string HandlerId { get; }

        public string Route { get; }

        public string Payload { get; }

        public SerializedEvent(Guid id, string eventId, string handlerId, string route, string payload)
        {
            Id = id;
            EventId = eventId;
            HandlerId = handlerId;
            Route = route;
            Payload = payload;
        }
    }
}
