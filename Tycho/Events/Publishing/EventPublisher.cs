using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Broker;
using Tycho.Events.Outbox;
using Tycho.Utils;

namespace Tycho.Events.Publishing
{
    internal class EventPublisher : IEventPublisher
    {
        private readonly IEventBroker _broker;
        private readonly IOutboxWriter _outbox;

        public EventPublisher(IEventBroker broker, IOutboxWriter outbox)
        {
            _broker = broker;
            _outbox = outbox;
        }

        async Task IEventPublisher.PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken)
        {
            eventPayload.ThrowIfNull();
            var eventId = Guid.NewGuid();
            var routedEvents = _broker.Route(eventId, eventPayload);
            if (routedEvents != null && routedEvents.Count > 0)
            {
                await _outbox.Write(routedEvents, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
