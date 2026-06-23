using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// Base interface for all Event Handlers.
    /// </summary>
    public interface IEventHandler
    {
    }

    /// <summary>
    /// Event Handler for an Event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Event to handle.</typeparam>
    public interface IEventHandler<TEvent> : IEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Handles an Event of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="context">The data of the Event to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task HandleAsync(EventContext<TEvent> context, CancellationToken cancellationToken);
    }
}
