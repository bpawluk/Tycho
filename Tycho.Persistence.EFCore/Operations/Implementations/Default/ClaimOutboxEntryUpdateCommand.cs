using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class ClaimOutboxEntryUpdateCommand
{
    public static async Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken)
    {
        Expression<Func<OutboxEntry, bool>> canBeProcessed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.Attempts < request.MaxAttempts) ||
            (entry.State == EntryState.InProcessing && entry.Attempts < request.MaxAttempts && entry.ClaimExpiration < request.UtcNow);

        return await dbContext
            .Set<OutboxEntry>()
            .Where(entry => entry.OwnerId == owner.Identifier)
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Updated)
            .ThenBy(entry => entry.EntryId)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.Updated, request.UtcNow)
                .SetProperty(entry => entry.Attempts, entry => entry.Attempts + 1)
                .SetProperty(entry => entry.ClaimId, request.ClaimId)
                .SetProperty(entry => entry.ClaimExpiration, request.Expiration), cancellationToken)
            .ConfigureAwait(false);
    }
}
