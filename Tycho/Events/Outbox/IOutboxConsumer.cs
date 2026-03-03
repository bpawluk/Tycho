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

        Task MarkAsDelivered(Guid entryId, CancellationToken cancellationToken = default);

        Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken = default);
    }
}
