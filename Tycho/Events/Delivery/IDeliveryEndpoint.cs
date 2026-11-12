using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;

namespace Tycho.Events.Delivery
{
    internal interface IDeliveryEndpoint
    {
        Task AcceptAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken);
    }
}
