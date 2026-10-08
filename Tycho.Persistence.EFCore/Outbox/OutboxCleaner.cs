using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Outbox;

internal sealed class OutboxCleaner(TychoDbContext dbContext, PersistenceOwner owner) : IOutboxCleaner
{
    public Task<int> CleanPayloadsAsync(DateTime cutoff, CancellationToken cancellationToken)
    {
        return ProcessedEntries()
            .Where(entry =>
                entry.Updated < cutoff &&
                entry.Payload != "{}")
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(entry => entry.Payload, "{}"), cancellationToken);
    }

    public Task<int> CleanEntriesAsync(DateTime cutoff, CancellationToken cancellationToken)
    {
        return ProcessedEntries()
            .Where(entry => entry.Updated < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private IQueryable<OutboxEntry> ProcessedEntries()
    {
        return dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.OwnerId == owner.Identifier &&
                entry.State == EntryState.Processed);
    }
}
