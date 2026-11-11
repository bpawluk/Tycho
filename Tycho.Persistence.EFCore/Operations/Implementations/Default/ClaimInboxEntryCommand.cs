using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class ClaimInboxEntryCommand
{
    public static async Task<ClaimResult<InboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken)
    {
        int count = await ClaimInboxEntryUpdateCommand
            .ExecuteAsync(dbContext, owner, request, cancellationToken)
            .ConfigureAwait(false);

        if (count != 1)
        {
            return ClaimResult<InboxEntry>.NotClaimed();
        }

        InboxEntry? entry = await ReadClaimedInboxEntryQuery
            .ExecuteAsync(dbContext, owner, request.ClaimId, cancellationToken)
            .ConfigureAwait(false);

        return entry is null ? ClaimResult<InboxEntry>.ClaimedButMissing() : ClaimResult<InboxEntry>.Claimed(entry);
    }
}
