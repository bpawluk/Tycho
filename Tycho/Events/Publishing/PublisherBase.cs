using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils;

namespace Tycho.Events.Publishing
{
    /// <summary>
    /// Base class for generated publishers for Tycho Events.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class PublisherBase
    {
        private readonly IEventPublisher _genericPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherBase"/> class.
        /// </summary>
        /// <param name="genericPublisher">The underlying Event publisher.</param>
        [ReferencedBySourceGenerator]
        public PublisherBase(IEventPublisher genericPublisher)
        {
            _genericPublisher = genericPublisher;
        }

        /// <summary>
        /// Publishes an Event.
        /// </summary>
        /// <typeparam name="TEvent">The Event type.</typeparam>
        /// <param name="eventPayload">The Event payload to publish.</param>
        /// <param name="cancellationToken">A token that can cancel Event publishing.</param>
        /// <returns>A task that completes when the Event has been published.</returns>
        [ReferencedBySourceGenerator]
        protected Task PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent
        {
            eventPayload.ThrowIfNull();
            return _genericPublisher.PublishAsync(eventPayload, cancellationToken);
        }
    }
}
