using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxConsumer
    {
        Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsDelivered(Guid eventId, CancellationToken cancellationToken = default);

        Task MarkAsFailed(Guid eventId, CancellationToken cancellationToken = default);
    }
}
