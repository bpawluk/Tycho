using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxWriter(InboxActivity inboxActivity, TychoDbContext dbContext, PersistenceOwner owner) : IInboxWriter
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly InboxActivity _inboxActivity = inboxActivity;

    public async Task Write(SerializedEvent serializedEvent, CancellationToken cancellationToken = default)
    {
        var inboxEntry = new InboxEntry
        {
            OwnerId = owner.Identifier,
            EntryId = serializedEvent.Id,
            PublishId = serializedEvent.PublishId,
            Handler = serializedEvent.HandlerId.ToString(),
            Event = serializedEvent.EventId.ToString(),
            Payload = serializedEvent.Payload.ToString()!
        };
        _dbContext.Set<InboxEntry>().Add(inboxEntry);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(inboxEntry).State = EntityState.Detached;

            InboxEntry? existing = await _dbContext
                .Set<InboxEntry>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x =>
                    x.OwnerId == owner.Identifier &&
                    x.EntryId == inboxEntry.EntryId, cancellationToken)
                .ConfigureAwait(false);

            if (existing is null ||
                existing.PublishId != inboxEntry.PublishId ||
                existing.Handler != inboxEntry.Handler ||
                existing.Event != inboxEntry.Event)
            {
                throw;
            }
        }

        _inboxActivity.NotifyNewEntriesAdded();
    }
}
