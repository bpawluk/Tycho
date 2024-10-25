using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Persistence;

namespace Tycho.Events.Publishing
{
    internal class EventPublisher : IEventPublisher, IUncommittedEventPublisher
    {
        private readonly IOutboxWriter _outbox;
        private readonly IEventRouter _router;
        private readonly IPayloadSerializer _serializer;

        public EventPublisher(IEventRouter router, IOutboxWriter persistentOutbox, IPayloadSerializer serializer)
        {
            _router = router;
            _outbox = persistentOutbox;
            _serializer = serializer;
        }

        public Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            return Publish(eventData, true, cancellationToken);
        }

        public Task PublishWithoutCommitting<TEvent>(TEvent eventData, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            return Publish(eventData, false, cancellationToken);
        }

        private async Task Publish<TEvent>(
            TEvent eventData,
            bool shouldCommit,
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData), $"{nameof(eventData)} cannot be null");
            }

            var handlerIdentities = _router.IdentifyHandlers<TEvent>();

            if (handlerIdentities.Count > 0)
            {
                var serializedPayload = _serializer.Serialize(eventData);
                var outboxEntries = handlerIdentities.Select(identity => new OutboxEntry(identity, serializedPayload));
                await _outbox.Write(outboxEntries.ToArray(), shouldCommit, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}