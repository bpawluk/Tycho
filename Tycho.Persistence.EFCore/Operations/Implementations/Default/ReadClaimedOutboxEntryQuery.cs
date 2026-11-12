using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class ReadClaimedOutboxEntryQuery
{
    public static Task<OutboxEntry?> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid claimId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Set<OutboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                entry =>
                    entry.OwnerId == owner.Identifier &&
                    entry.ClaimId == claimId,
                cancellationToken);
    }
}
