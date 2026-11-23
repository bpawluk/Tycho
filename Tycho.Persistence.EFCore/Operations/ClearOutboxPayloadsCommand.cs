using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Identity;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;

namespace Tycho.Persistence.EFCore.Operations;

internal static class ClearOutboxPayloadsCommand
{
    internal static Task<int> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        DateTime cutoff,
        CancellationToken cancellationToken = default)
    {
        return Default.ClearOutboxPayloadsCommand.ExecuteAsync(dbContext, owner, cutoff, cancellationToken);
    }
}
