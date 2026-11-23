using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;

namespace Tycho.Persistence.EFCore.Operations;

internal static class FindInboxEntryQuery
{
    internal static Task<InboxEntry?> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        Guid entryId,
        CancellationToken cancellationToken = default)
    {
        return Default.FindInboxEntryQuery.ExecuteAsync(dbContext, owner, entryId, cancellationToken);
    }
}
