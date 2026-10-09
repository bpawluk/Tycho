using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Structure;

namespace Tycho.Events.Delivery
{
    internal sealed class EventDeliverer : IEventDeliverer
    {
        private readonly ControlPlane _controlPlane;

        public EventDeliverer(ControlPlane controlPlane)
        {
            _controlPlane = controlPlane;
        }

        public async Task DeliverAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            await _controlPlane
                .GetModule(routedEvent.DestinationId)
                .Endpoint
                .AcceptAsync(routedEvent, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
