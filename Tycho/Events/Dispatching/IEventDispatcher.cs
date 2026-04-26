using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Dispatching
{
    internal interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(RoutedEvent<TEvent> @event, CancellationToken cancellationToken) 
            where TEvent : class, IEvent;
    }
}
