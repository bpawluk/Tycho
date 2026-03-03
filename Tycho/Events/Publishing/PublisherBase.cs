using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils;

namespace Tycho.Events.Publishing
{
    [ReferencedBySourceGenerator]
    public abstract class PublisherBase
    {
        private readonly IEventPublisher _genericPublisher;

        [ReferencedBySourceGenerator]
        public PublisherBase(IEventPublisher genericPublisher)
        {
            _genericPublisher = genericPublisher;
        }

        [ReferencedBySourceGenerator]
        protected Task PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken = default)
            where TEvent : class, IEvent
        {
            eventPayload.ThrowIfNull();
            return _genericPublisher.PublishAsync(eventPayload, cancellationToken);
        }
    }
}
