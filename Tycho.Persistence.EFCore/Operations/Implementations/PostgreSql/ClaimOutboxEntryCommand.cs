using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.PostgreSql;

internal static class ClaimOutboxEntryCommand
{
    public static Task<ClaimResult<OutboxEntry>> ExecuteAsync(
        TychoDbContext dbContext,
        PersistenceOwner owner,
        ClaimRequest request,
        CancellationToken cancellationToken) =>
        ClaimCommand.ExecuteAsync<OutboxEntry>(dbContext, new OutboxEntryTable(dbContext), owner, request, cancellationToken);
}
