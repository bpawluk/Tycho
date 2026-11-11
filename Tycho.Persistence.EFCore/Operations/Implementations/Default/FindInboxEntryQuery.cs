using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class FindInboxEntryQuery
{
    public static Task<InboxEntry?> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid entryId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Set<InboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                entry =>
                    entry.OwnerId == owner.Identifier &&
                    entry.EntryId == entryId,
                cancellationToken);
    }
}
