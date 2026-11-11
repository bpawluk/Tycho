using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class DeleteOutboxEntriesCommand
{
    public static Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        DateTime cutoff,
        CancellationToken cancellationToken)
    {
        return dbContext.Set<OutboxEntry>()
            .Where(entry =>
                entry.OwnerId == owner.Identifier &&
                entry.State == EntryState.Processed)
            .Where(entry => entry.Updated < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
