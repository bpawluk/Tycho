using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence
{
    internal interface IOutboxConsumer
    {
        Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken = default);

        Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken = default);
    }
}