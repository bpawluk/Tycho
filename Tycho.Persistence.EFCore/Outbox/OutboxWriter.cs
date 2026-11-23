using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Serialization;
using Tycho.Persistence.EFCore.Identity;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(
    IEventSerializer eventSerializer,
    OutboxActivity outboxActivity,
    TychoDbContext dbContext,
    PersistenceOwner owner) : IOutboxWriter
{
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly OutboxActivity _outboxActivity = outboxActivity;
    private readonly TychoDbContext _dbContext = dbContext;

    public async Task Write(IReadOnlyCollection<RoutedEvent> routedEvents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(routedEvents);
        cancellationToken.ThrowIfCancellationRequested();

        OutboxEntry[] outboxEntries = [.. routedEvents.Select(routedEvent =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            SerializedRoutedEvent serializedEvent = _eventSerializer.Serialize(routedEvent);

            return new OutboxEntry
            {
                OwnerId = owner.Identifier,
                EntryId = serializedEvent.Id,
                PublishId = serializedEvent.PublishId,
                Destination = serializedEvent.DestinationId.Value,
                Handler = serializedEvent.HandlerId.ToString(),
                Event = serializedEvent.EventId.ToString(),
                Payload = serializedEvent.Payload.ToString()!
            };
        })];

        cancellationToken.ThrowIfCancellationRequested();
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

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            foreach (OutboxEntry entry in outboxEntries)
            {
                _dbContext.Entry(entry).State = EntityState.Detached;
            }

            throw;
        }
        _outboxActivity.NotifyNewEntriesAdded();
    }
}
