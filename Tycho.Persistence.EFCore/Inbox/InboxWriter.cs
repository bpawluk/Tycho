using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxWriter(ITransaction transaction, InboxActivity inboxActivity, TychoDbContext dbContext) : IInboxWriter
{
    private readonly ITransaction _transaction = transaction;
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly InboxActivity _inboxActivity = inboxActivity;

    public async Task Write(SerializedRoutedEvent serializedEvent, CancellationToken cancellationToken = default)
    {
        var inboxEntry = new InboxEntry
        {
            Id = serializedEvent.Id,
            Event = serializedEvent.EventId.ToString(),
            Handler = serializedEvent.HandlerId.ToString(),
            Payload = serializedEvent.Payload.ToString()!
        };
        _dbContext.Set<InboxEntry>().Add(inboxEntry);

        if (!_transaction.IsInProgress)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _inboxActivity.NotifyNewEntriesAdded();
    }
}