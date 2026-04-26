using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Identity.Events;

namespace Tycho.Events.Dispatching
{
    internal class EventDispatcher : IEventDispatcher
    {
        private readonly IEventHandlerProvider _handlerProvider;

        public EventDispatcher(IEventHandlerProvider handlerProvider)
        {
            _handlerProvider = handlerProvider;
        }

        public async Task DispatchAsync<TEvent>(RoutedEvent<TEvent> @event, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            var handler = _handlerProvider.GetHandler<TEvent>(@event.HandlerId);
            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"No handler found for event of type {typeof(TEvent).Name} " +
                    $"with handler ID {@event.HandlerId}.");
            }

            var context = new EventContext<TEvent>(@event.Id, @event.Payload);
            await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
