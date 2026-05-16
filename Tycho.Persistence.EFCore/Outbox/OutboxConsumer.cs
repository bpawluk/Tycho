using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(ITransaction transaction, TychoDbContext dbContext, OutboxConsumerSettings? settings = null) : IOutboxConsumer
{
    private readonly ITransaction _transaction = transaction;
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxConsumerSettings _settings = settings ?? OutboxConsumerSettings.Default;

    // TODO: concurrency handling
    // TODO: dead letter handling

    public async Task<IReadOnlyCollection<SerializedRoutedEvent>> Read(int count, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;
        DateTime validProcessingThreshold = currentTime - _settings.DeliveryExpiration;

        OutboxEntry[] entriesToDeliver = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                (entry.State == EntryState.New) ||
                (entry.State == EntryState.Failed && entry.DeliveryAttempts < _settings.MaxDeliveryCount) ||
                (entry.State == EntryState.InProcessing && entry.DeliveryAttempts < _settings.MaxDeliveryCount && entry.Updated < validProcessingThreshold))
            .OrderBy(entry => entry.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (OutboxEntry? entry in entriesToDeliver)
        {
            entry.State = EntryState.InProcessing;
            entry.Updated = currentTime;
            entry.DeliveryAttempts++;
        }

        if (!_transaction.IsInProgress)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return
        [
            ..entriesToDeliver
                .Select(entry => new SerializedRoutedEvent(
                    entry.Id,
                    EventIdentity.Parse(entry.Event),
                    EventHandlerIdentity.Parse(entry.Handler),
                    Route.Parse(entry.Route),
                    entry.Payload))
        ];
    }

    public async Task MarkAsDelivered(Guid entryId, CancellationToken cancellationToken)
    {
        DbSet<OutboxEntry> outboxMessages = _dbContext.Set<OutboxEntry>();
        OutboxEntry? entry = await outboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            entry.State = EntryState.Processed;
            entry.Updated = DateTime.UtcNow;

            if (!_transaction.IsInProgress)
            {
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public async Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken)
    {
        DbSet<OutboxEntry> outboxMessages = _dbContext.Set<OutboxEntry>();
        OutboxEntry? entry = await outboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            entry.State = EntryState.Failed;
            entry.Updated = DateTime.UtcNow;

            if (!_transaction.IsInProgress)
            {
                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
