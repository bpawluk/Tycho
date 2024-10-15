using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Persistence;

namespace Tycho.Events.Publishing
{
    internal class EventPublisher : IEventPublisher
    {
        private readonly IOutbox _outbox;
        private readonly IEventRouter _router;
        private readonly IPayloadSerializer _serializer;

        public EventPublisher(IEventRouter router, IOutbox persistentOutbox, IPayloadSerializer serializer)
        {
            _router = router;
            _outbox = persistentOutbox;
            _serializer = serializer;
        }

        public async Task Publish<TEvent>(
            TEvent eventData,
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var handlerIdentities = _router.IdentifyHandlers<TEvent>();

            if (handlerIdentities.Count > 0)
            {
                await AddOutboxEntries(handlerIdentities, eventData, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task AddOutboxEntries(IReadOnlyCollection<HandlerIdentity> handlerIdentities, IEvent eventData,
            CancellationToken cancellationToken)
        {
            var serializedPayload = _serializer.Serialize(eventData);
            var outboxEntries = handlerIdentities.Select(identity => new OutboxEntry(identity, serializedPayload));
            await _outbox.Add(outboxEntries.ToArray(), cancellationToken).ConfigureAwait(false);
        }
    }
}