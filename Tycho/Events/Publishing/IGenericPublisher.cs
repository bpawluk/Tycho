using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Publishing
{
    /// <summary>
    /// An interface for publishing events.
    /// </summary>
    public interface IGenericPublisher
    {
        /// <summary>
        /// Publishes an event of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to publish.</typeparam>
        /// <param name="eventPayload">The data of the event to publish.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}
