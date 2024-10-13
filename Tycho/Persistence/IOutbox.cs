using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence
{
    internal interface IOutbox
    {
        event EventHandler NewEntriesAdded;

        Task Add(IReadOnlyCollection<OutboxEntry> entries, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken = default);

        Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken = default);

        Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken = default);
    }
}
