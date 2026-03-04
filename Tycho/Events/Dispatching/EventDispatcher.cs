using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
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

        public async Task DispatchAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            var handler = _handlerProvider.GetHandler<TEvent>(routedEvent.HandlerId);
            var context = new EventContext<TEvent>(routedEvent.Id, routedEvent.Payload);
            await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
