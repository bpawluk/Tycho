using System;
using Tycho.Utils;

namespace Tycho.Events
{
    /// <summary>
    /// Represents an event being handled by an <see cref="IEventHandler{TEvent}"/> together with its metadata.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event payload.</typeparam>
    [ReferencedBySourceGenerator]
    public class EventContext<TEvent> where TEvent : class, IEvent
    {
        /// <summary>
        /// Gets the unique identifier of the event instance.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the payload of the event instance.
        /// </summary>
        public TEvent Payload { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventContext{TEvent}"/> class.
        /// </summary>
        /// <param name="id">The event identifier.</param>
        /// <param name="payload">The event payload.</param>
        [ReferencedBySourceGenerator]
        public EventContext(Guid id, TEvent payload)
        {
            Id = id;
            Payload = payload;
        }
    }
}
