using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(TychoDbContext dbContext, OutboxConsumerSettings? settings = null) : IOutboxConsumer
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxConsumerSettings _settings = settings ?? OutboxConsumerSettings.Default;

    public async Task<OutboxEvent?> TryReadAsync(CancellationToken cancellationToken)
    {
        Guid claimId = Guid.NewGuid();
        DateTime utcNow = DateTime.UtcNow;

        Expression<Func<OutboxEntry, bool>> canBeProcessed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.DeliveryAttempts < _settings.MaxDeliveryCount) ||
            (entry.State == EntryState.InProcessing && entry.DeliveryAttempts < _settings.MaxDeliveryCount && entry.ClaimExpiration < utcNow);

        int claimedEntries = await _dbContext
            .Set<OutboxEntry>()
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Updated)
            .ThenBy(entry => entry.Id)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, utcNow)
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.DeliveryAttempts, entry => entry.DeliveryAttempts + 1)
                .SetProperty(entry => entry.ClaimId, claimId)
                .SetProperty(entry => entry.ClaimExpiration, utcNow + _settings.DeliveryExpiration), cancellationToken)
            .ConfigureAwait(false);

        if (claimedEntries != 1)
        {
            return null;
        }

        OutboxEntry? entryToDeliver = await _dbContext
            .Set<OutboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(entry => entry.ClaimId == claimId, cancellationToken)
            .ConfigureAwait(false);

        return entryToDeliver == null
            ? null
            : new OutboxEvent(
                claimId,
                new SerializedRoutedEvent(
                    entryToDeliver.Id,
                    entryToDeliver.PublishId,
                    EventIdentity.Parse(entryToDeliver.Event),
                    EventHandlerIdentity.Parse(entryToDeliver.Handler),
                    Route.Parse(entryToDeliver.Route),
                    entryToDeliver.Payload));
    }

    public async Task<bool> MarkAsDeliveredAsync(Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRowsCount = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.State, EntryState.Processed)
                .SetProperty(entry => entry.ClaimId, Guid.Empty)
                .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue), cancellationToken)
            .ConfigureAwait(false);

        return updatedRowsCount == 1;
    }

    public async Task<bool> MarkAsFailedAsync(Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRowsCount = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.State, EntryState.Failed)
                .SetProperty(entry => entry.ClaimId, Guid.Empty)
                .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue), cancellationToken)
            .ConfigureAwait(false);

        return updatedRowsCount == 1;
    }
}
