using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Inbox;

internal sealed class InboxCleaner(TychoDbContext dbContext, PersistenceOwner owner) : IInboxCleaner
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

    private IQueryable<InboxEntry> ProcessedEntries()
    {
        return dbContext
            .Set<InboxEntry>()
            .Where(entry =>
                entry.OwnerId == owner.Identifier &&
                entry.State == EntryState.Processed);
    }
}
