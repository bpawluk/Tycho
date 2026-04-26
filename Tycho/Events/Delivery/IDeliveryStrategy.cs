using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Delivery
{
    internal interface IDeliveryStrategy
    {
        bool CanDeliver(SerializedRoutedEvent routedEvent);

        Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
