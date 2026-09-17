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

    public async Task Write(SerializedRoutedEvent serializedEvent, CancellationToken cancellationToken = default)
    {
        var inboxEntry = new InboxEntry
        {
            OwnerKey = owner.Key,
            Id = serializedEvent.Id,
            PublishId = serializedEvent.PublishId,
            Event = serializedEvent.EventId.ToString(),
            Handler = serializedEvent.HandlerId.ToString(),
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
                    x.OwnerKey == owner.Key &&
                    x.Id == inboxEntry.Id, cancellationToken)
                .ConfigureAwait(false);

            if (existing is null ||
                existing.PublishId != inboxEntry.PublishId ||
                existing.Event != inboxEntry.Event ||
                existing.Handler != inboxEntry.Handler ||
                existing.Payload != inboxEntry.Payload)
            {
                throw;
            }
        }

        _inboxActivity.NotifyNewEntriesAdded();
    }
}
