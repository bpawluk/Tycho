using System.Threading;
using System.Threading.Tasks;
using Tycho.Persistence.EFCore.Inbox;
using Default = Tycho.Persistence.EFCore.Operations.Implementations.Default;

namespace Tycho.Persistence.EFCore.Operations;

internal static class InsertInboxEntryCommand
{
    internal static Task ExecuteAsync(
        TychoDbContext dbContext,
        InboxEntry entry,
        CancellationToken cancellationToken = default)
    {
        return Default.InsertInboxEntryCommand.ExecuteAsync(dbContext, entry, cancellationToken);
    }
}
