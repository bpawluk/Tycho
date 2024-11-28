using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// An interface for publishing events
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes an event of type <typeparamref name="TEvent"/>
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to publish</typeparam>
        /// <param name="eventData">The data of the event to publish</param>
        Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent;
    }
}