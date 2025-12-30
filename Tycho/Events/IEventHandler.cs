using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// Base interface for all event handlers
    /// </summary>
    public interface IEventHandler
    {
        // TODO: Remove
        internal Type EventType { get; }
    }

    /// <summary>
    /// Event handler for an event of type <typeparamref name="TEvent"/>
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle</typeparam>
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        Type IEventHandler.EventType => typeof(TEvent);

        /// <summary>
        /// Handles an event of type <typeparamref name="TEvent"/>
        /// </summary>
        /// <param name="context">The data of the event to handle</param>
        Task Handle(EventContext<TEvent> context, CancellationToken cancellationToken);
    }
}
