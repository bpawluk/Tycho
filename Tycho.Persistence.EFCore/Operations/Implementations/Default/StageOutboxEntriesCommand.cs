using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class StageOutboxEntriesCommand
{
    public static Task ExecuteAsync(
        TychoDbContext dbContext,
        IReadOnlyCollection<OutboxEntry> entries,
        CancellationToken _)
    {
        dbContext.Set<OutboxEntry>().AddRange(entries);
        return Task.CompletedTask;
    }
}
