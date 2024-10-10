using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Routing;
using TychoV2.Persistence;
using TychoV2.Structure;

namespace TychoV2.Events.Broker
{
    internal class EventBroker : IPublish, IEventProcessor
    {
        private readonly IEventRouter _router;
        private readonly IOutbox _outbox;

        public EventBroker(Internals internals, IOutbox persistentOutbox)
        {
            _router = new EventRouter(internals);
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

            var handlerIdentities = _router.IdentifyHandlers<TEvent>();
            if (handlerIdentities.Count > 0)
            {
                var outboxEntries = handlerIdentities.Select(identity => new Entry(identity, eventData));
                await _outbox.Add(outboxEntries.ToArray(), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<bool> Process<TEvent>(
            HandlerIdentity handlerIdentity,
            TEvent eventData, 
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            try
            {
                if (eventData is null)
                {
                    throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
                }

                var eventHandler = _router.FindHandler<TEvent>(handlerIdentity);
                await eventHandler!.Handle(eventData, cancellationToken).ConfigureAwait(false);

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
