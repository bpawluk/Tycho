using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Inbox;

namespace Tycho.Persistence.EFCore.Operations.Implementations.Default;

internal static class InsertInboxEntryCommand
{
    public static async Task ExecuteAsync(TychoDbContext dbContext, InboxEntry entry, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        dbContext.Set<InboxEntry>().Add(entry);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            dbContext.Entry(entry).State = EntityState.Detached;
            throw;
        }
    }
}
