using System;

namespace Tycho.Events
{
    /// <summary>
    /// Represents an Event being handled by an <see cref="IEventHandler{TEvent}"/> together with its metadata.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Event payload.</typeparam>
    public class EventContext<TEvent> where TEvent : class, IEvent
    {
        /// <summary>
        /// Gets the unique identifier of the Event instance.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the payload of the Event instance.
        /// </summary>
        public TEvent Payload { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContext{TEvent}"/> class.
        /// </summary>
        /// <param name="id">The Event identifier.</param>
        /// <param name="payload">The Event payload.</param>
        public EventContext(Guid id, TEvent payload)
        {
            Id = id;
            Payload = payload;
        }
    }
}
