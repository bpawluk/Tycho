using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils;

namespace Tycho.Events
{
    /// <summary>
    /// Base interface for all event handlers.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IEventHandler
    {
    }

    /// <summary>
    /// Event handler for an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Handles an event of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="context">The data of the event to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [ReferencedBySourceGenerator]
        Task HandleAsync(EventContext<TEvent> context, CancellationToken cancellationToken);
    }
}
