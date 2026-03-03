using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Dispatching
{
    internal interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken) 
            where TEvent : class, IEvent;
    }
}
