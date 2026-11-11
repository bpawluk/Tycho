using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxWriter(
    InboxActivity inboxActivity,
    TychoDbContext dbContext,
    PersistenceOwner owner) : IInboxWriter
{
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

        try
        {
            await InsertInboxEntryCommand.ExecuteAsync(dbContext, inboxEntry, cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateException)
        {
            InboxEntry? existing = await FindInboxEntryQuery.ExecuteAsync(dbContext, owner, inboxEntry.EntryId, cancellationToken).ConfigureAwait(false);

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
