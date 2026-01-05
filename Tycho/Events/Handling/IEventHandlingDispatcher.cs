using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Handling
{
    /// <summary>
    /// Dispatches events to respective event handlers.
    /// </summary>
    public interface IEventHandlingDispatcher
    {
        /// <summary>
        /// Dispatches the specified event to the provided <paramref name="eventHandler"/>.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event instance.</param>
        /// <param name="eventPayload">The event payload of the event instance.</param>
        /// <param name="eventHandler">The handler that will process the event.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task Dispatch(
            Guid eventId,
            object eventPayload,
            IEventHandler eventHandler,
            CancellationToken cancellationToken);
    }
}
