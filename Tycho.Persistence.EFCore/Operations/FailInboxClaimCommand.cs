using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Identity;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;

namespace Tycho.Persistence.EFCore.Operations;

internal static class FailInboxClaimCommand
{
    internal static Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid entryId,
        Guid claimId,
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        return Default.FailInboxClaimCommand.ExecuteAsync(dbContext, owner, entryId, claimId, utcNow, cancellationToken);
    }
}
