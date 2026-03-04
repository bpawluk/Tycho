using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Delivery
{
    internal interface IDeliveryStrategy
    {
        bool CanDeliver<TEvent>(RoutedEvent<TEvent> routedEvent) 
            where TEvent : class, IEvent;

        Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken) 
            where TEvent : class, IEvent;
    }
}
