using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Outbox;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;

namespace Tycho.Persistence.EFCore.Operations;

internal static class StageOutboxEntriesCommand
{
    internal static Task ExecuteAsync(
        TychoDbContext dbContext,
        IReadOnlyCollection<OutboxEntry> entries,
        CancellationToken cancellationToken = default)
    {
        return Default.StageOutboxEntriesCommand.ExecuteAsync(dbContext, entries, cancellationToken);
    }
}
