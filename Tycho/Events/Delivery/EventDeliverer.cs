using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Structure;
using Tycho.Utils;

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
            routedEvent.ThrowIfNull();
            cancellationToken.ThrowIfCancellationRequested();
            await _controlPlane
                .GetModule(routedEvent.DestinationId)
                .Endpoint
                .AcceptAsync(routedEvent, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
