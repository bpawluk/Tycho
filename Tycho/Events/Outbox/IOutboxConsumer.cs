using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxConsumer
    {
        Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsDelivered(OutboxEntry entry, CancellationToken cancellationToken = default);

        Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken = default);
    }
}
