using System;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Outbox;
using TychoV2.Events.Router;

namespace TychoV2.Events.Broker
{
    internal class EventBroker<TEvent> : IPublish<TEvent>, IProcess<TEvent>
        where TEvent : class, IEvent
    {
        private readonly EventRouter _router;
        private readonly IOutbox _outbox;

        public EventBroker(EventRouter eventRouter, IOutbox persistentOutbox)
        {
            _router = eventRouter;
            _outbox = persistentOutbox;
        }

        public async Task Publish(TEvent eventData, CancellationToken cancellationToken)
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var eventHandlerIds = _router.IdentifyEventHandlers<TEvent>();

            if (eventHandlerIds.Count > 0)
            {
                await _outbox.Enqueue(eventHandlerIds, eventData, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<bool> Process(
            string handlerId,
            TEvent eventData,
            CancellationToken cancellationToken)
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var eventHandler = _router.GetEventHandler<TEvent>(handlerId);

            try
            {
                await eventHandler.Handle(eventData, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                // TODO: log
                return false;
            }
        }
    }
}
