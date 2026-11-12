using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxConsumer
    {
        Task<OutboxEvent?> TryReadAsync(CancellationToken cancellationToken = default);

        Task<bool> MarkAsDeliveredAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken = default);

        Task<bool> MarkAsFailedAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken = default);
    }
}
