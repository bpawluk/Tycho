using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events.Outbox
{
    internal interface IOutboxWriter
    {
        Task WriteUncommitted(IReadOnlyList<OutboxEntry> entries, CancellationToken cancellationToken = default);

        Task WriteAndCommit(IReadOnlyList<OutboxEntry> entries, CancellationToken cancellationToken = default);
    }
}
