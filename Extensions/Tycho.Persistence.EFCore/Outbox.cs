using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore;

internal class Outbox : IOutbox
{
    public event EventHandler? NewEntriesAdded;

    public Task Add(IReadOnlyCollection<OutboxEntry> entries, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}