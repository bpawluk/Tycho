using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Delivery
{
    internal sealed class DeliveryEndpoint : IDeliveryEndpoint
    {
        private readonly Internals _internals;

        public DeliveryEndpoint(Internals internals)
        {
            _internals = internals;
        }

        [EntryPoint]
        public async Task AcceptAsync(SerializedRoutedEvent routedEvent, CancellationToken cancellationToken)
        {
            if (routedEvent.DestinationId != _internals.OwnerId)
            {
                throw new InvalidOperationException(
                    $"Routed event destination '{routedEvent.DestinationId}' " +
                    $"does not match '{_internals.OwnerId}' endpoint.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IInboxWriter inbox = scope.ServiceProvider.GetRequiredService<IInboxWriter>();
            await inbox.Write(routedEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
