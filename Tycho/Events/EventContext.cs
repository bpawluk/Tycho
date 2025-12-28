using System;

namespace Tycho.Events
{
    public class EventContext<TEvent> where TEvent : class, IEvent
    {
        public Guid Id { get; set; }

        public TEvent Payload { get; set; }

        public EventContext(Guid id, TEvent payload)
        {
            Id = id;
            Payload = payload;
        }
    }
}
