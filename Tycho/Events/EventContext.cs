using System;

namespace Tycho.Events
{
    public class EventContext<TEvent> where TEvent : class, IEvent
    {
        public Guid Id { get; set; }

        public TEvent Event { get; set; }

        public EventContext(Guid id, TEvent @event)
        {
            Id = id;
            Event = @event;
        }
    }
}
