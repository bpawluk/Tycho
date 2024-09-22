using NewTycho.Events.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Events.Publishing
{
    internal class EventBroker
    {
        private readonly EventRouter _eventRouter;
        private readonly IPersistentOutbox _persistentOutbox;

        public EventBroker(EventRouter eventRouter, IPersistentOutbox persistentOutbox)
        {
            _eventRouter = eventRouter;
            _persistentOutbox = persistentOutbox;
        }

        public async Task Publish<TEvent>(TEvent eventData, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var eventHandlerIds = _eventRouter.IdentifyEventHandlers<TEvent>();

            if (eventHandlerIds.Length > 0)
            {
                await _persistentOutbox.Enqueue(eventHandlerIds, eventData, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<bool> Process<TEvent>(string handlerId, TEvent eventData, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            var eventHandler = _eventRouter.GetEventHandler<TEvent>(handlerId);

            try
            {
                await eventHandler.Handle(eventData, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                // log
                return false;
            }
        }
    }
}
