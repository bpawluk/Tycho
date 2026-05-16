using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(
    ITransaction transaction,
    IEventSerializer eventSerializer,
    OutboxActivity outboxActivity,
    TychoDbContext dbContext) : IOutboxWriter
{
    private readonly ITransaction _transaction = transaction;
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly OutboxActivity _outboxActivity = outboxActivity;
    private readonly TychoDbContext _dbContext = dbContext;

    public async Task Write(IReadOnlyCollection<RoutedEvent> routedEvents, CancellationToken cancellationToken)
    {
        IEnumerable<OutboxEntry> outboxEntries = routedEvents.Select(routedEvent =>
        {
            SerializedRoutedEvent serializedEvent = _eventSerializer.Serialize(routedEvent);
            return new OutboxEntry
            {
                Id = serializedEvent.Id,
                Event = serializedEvent.EventId.ToString(),
                Handler = serializedEvent.HandlerId.ToString(),
                Route = serializedEvent.Route.ToString(),
                Payload = serializedEvent.Payload.ToString()!
            };
        });
        _dbContext.Set<OutboxEntry>().AddRange(outboxEntries);

        if (!_transaction.IsInProgress)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _outboxActivity.NotifyNewEntriesAdded();
    }
}
