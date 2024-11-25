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
        /// <summary>
        /// Type of the event to handle
        /// </summary>
        Type EventType { get; }

        /// <summary>
        /// The type of the event handler
        /// </summary>
        Type HandlerType { get; }
    }

    /// <summary>
    /// Event handler for an event of type <typeparamref name="TEvent"/>
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle</typeparam>
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        /// <inheritdoc />
        Type IEventHandler.EventType => typeof(TEvent);

        /// <inheritdoc />
        Type IEventHandler.HandlerType => GetType();

        /// <summary>
        /// Handles an event of type <typeparamref name="TEvent"/>
        /// </summary>
        /// <param name="eventData">The data of the event to handle</param>
        Task Handle(TEvent eventData, CancellationToken cancellationToken = default);
    }
}