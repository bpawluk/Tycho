using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing.Payload;

namespace Tycho.Events.Routing.Delivery
{
    internal interface IDeliveryStrategy
    {
        Task DeliverAsync(IRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
