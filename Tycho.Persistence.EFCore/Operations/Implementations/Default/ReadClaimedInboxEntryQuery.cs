using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class ReadClaimedInboxEntryQuery
{
    public static Task<InboxEntry?> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid claimId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Set<InboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                entry =>
                    entry.OwnerId == owner.Identifier &&
                    entry.ClaimId == claimId,
                cancellationToken);
    }
}
