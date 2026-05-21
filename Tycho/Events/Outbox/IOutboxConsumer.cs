using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxConsumer
    {
        Task<IReadOnlyCollection<OutboxEvent>> Read(int count, CancellationToken cancellationToken = default);

        Task<bool> MarkAsDelivered(Guid eventId, Guid claimId, CancellationToken cancellationToken = default);

        Task<bool> MarkAsFailed(Guid eventId, Guid claimId, CancellationToken cancellationToken = default);
    }
}
