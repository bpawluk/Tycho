using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;

namespace Tycho.Events.Publishing
{
    internal class GenericPublisher : IGenericPublisher
    {
        private readonly IEventRouter _router;
        private readonly IPayloadSerializer _serializer;
        private readonly IOutboxWriter _outbox;

        public GenericPublisher(IEventRouter router, IPayloadSerializer serializer, IOutboxWriter outbox)
        {
            _router = router;
            _serializer = serializer;
            _outbox = outbox;
        }

        async Task IGenericPublisher.PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken)
        {
            var eventId = Guid.NewGuid();
            var routedEvents = _router.FindRoutes(eventId, eventPayload);

            var outboxEntries = routedEvents
                .Select(routedEvent =>
                    new OutboxEntry(
                        routedEvent.Id,
                        _serializer.Serialize(routedEvent.Payload),
                        routedEvent.Route))
                .ToList();

            await _outbox.Write(outboxEntries, cancellationToken).ConfigureAwait(false);
        }
    }
}
