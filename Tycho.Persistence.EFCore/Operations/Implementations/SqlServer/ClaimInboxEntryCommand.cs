using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.SqlServer;

internal static class ClaimInboxEntryCommand
{
    public static Task<ClaimResult<InboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken) =>
        ClaimCommand.ExecuteAsync<InboxEntry>(dbContext, new InboxEntryTable(dbContext), owner, request, cancellationToken);
}
