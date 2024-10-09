using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Routing;
using TychoV2.Persistence;

namespace TychoV2.Events.Broker
{
    internal class EventBroker : IPublish, IEventProcessor
    {
        private readonly EventRouter _router;
        private readonly IOutbox _outbox;

        public EventBroker(EventRouter eventRouter, IOutbox persistentOutbox)
        {
            _router = eventRouter;
            _outbox = persistentOutbox;
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

            var eventHandlerIds = _router.IdentifyHandlers<TEvent>();

            if (eventHandlerIds.Count > 0)
            {
                var outboxEntries = eventHandlerIds.Select(handlerId => new Entry(handlerId, eventData));
                await _outbox.Add(outboxEntries.ToArray(), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<bool> Process<TEvent>(
            HandlerIdentity handlerIdentity,
            TEvent eventData, 
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var eventHandler = _router.GetHandler<TEvent>(handlerIdentity);

            try
            {
                await eventHandler.Handle(eventData, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch
            {
                // TODO: Log the Exception
                return false;
            }
        }
    }
}
