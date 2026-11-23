using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class FailOutboxClaimCommand
{
    public static async Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid entryId,
        Guid claimId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.OwnerId == owner.Identifier &&
                entry.EntryId == entryId &&
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(entry => entry.State, EntryState.Failed)
                    .SetProperty(entry => entry.Updated, utcNow)
                    .SetProperty(entry => entry.ClaimId, Guid.Empty)
                    .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
