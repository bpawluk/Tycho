using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.MySql;

internal static class ClaimOutboxEntryCommand
{
    public static async Task<ClaimResult<OutboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken)
    {
        int count = await ClaimCommand
            .ExecuteAsync(dbContext, new OutboxEntryTable(dbContext), owner, request, cancellationToken)
            .ConfigureAwait(false);

        if (count != 1)
        {
            return ClaimResult<OutboxEntry>.NotClaimed();
        }

        OutboxEntry? entry = await Default.ReadClaimedOutboxEntryQuery
            .ExecuteAsync(dbContext, owner, request.ClaimId, cancellationToken)
            .ConfigureAwait(false);

        return entry is null ? ClaimResult<OutboxEntry>.ClaimedButMissing() : ClaimResult<OutboxEntry>.Claimed(entry);
    }
}
