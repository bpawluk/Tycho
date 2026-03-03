using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Delivery
{
    internal interface IDeliveryStrategy
    {
        Task DeliverAsync<TEvent>(RoutedEvent<TEvent> routedEvent, CancellationToken cancellationToken) 
            where TEvent : class, IEvent;
    }
}
