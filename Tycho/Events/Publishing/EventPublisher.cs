using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;

namespace Tycho.Events.Publishing
{
    internal class EventPublisher : IEventPublisher
    {
        private readonly IEventRouter _router;
        private readonly IOutboxWriter _outbox;

        public EventPublisher(IEventRouter router, IOutboxWriter outbox)
        {
            _router = router;
            _outbox = outbox;
        }

        async Task IEventPublisher.PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken)
        {
            var eventId = Guid.NewGuid();
            var routedEvents = _router.Route(eventId, eventPayload);
            await _outbox.Write(routedEvents, cancellationToken).ConfigureAwait(false);
        }
    }
}
