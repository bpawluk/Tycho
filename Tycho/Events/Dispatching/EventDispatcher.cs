using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Dispatching
{
    internal class EventDispatcher : IEventDispatcher
    {
        public EventDispatcher()
        {
        }

        public async Task DispatchAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
