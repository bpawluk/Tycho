using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(
    IEventSerializer eventSerializer,
    OutboxActivity outboxActivity,
    TychoDbContext dbContext) : IOutboxWriter
{
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly OutboxActivity _outboxActivity = outboxActivity;
    private readonly TychoDbContext _dbContext = dbContext;

    public async Task Write(IReadOnlyCollection<RoutedEvent> routedEvents, CancellationToken cancellationToken)
    {
        OutboxEntry[] outboxEntries = [.. routedEvents.Select(routedEvent =>
        {
            SerializedRoutedEvent serializedEvent = _eventSerializer.Serialize(routedEvent);
            return new OutboxEntry
            {
                Id = serializedEvent.Id,
                PublishId = serializedEvent.PublishId,
                Event = serializedEvent.EventId.ToString(),
                Handler = serializedEvent.HandlerId.ToString(),
                Route = serializedEvent.Route.ToString(),
                Payload = serializedEvent.Payload.ToString()!
            };
        })];

        _dbContext.Set<OutboxEntry>().AddRange(outboxEntries);

        if (System.Transactions.Transaction.Current is not null)
        {
            return;
        }

        if (_dbContext.Database.CurrentTransaction is IDbContextTransaction transaction)
        {
            _dbContext.TransactionInterceptor.ExecuteAfterCommit(transaction.TransactionId, _outboxActivity.NotifyNewEntriesAdded);
            return;
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        _outboxActivity.NotifyNewEntriesAdded();
    }
}
