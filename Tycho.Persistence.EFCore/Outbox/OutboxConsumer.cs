using System;
using System.Collections.Generic;
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

    public async Task<IReadOnlyCollection<SerializedRoutedEvent>> Read(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return [];
        }

        DateTime currentTime = DateTime.UtcNow;
        DateTime validProcessingThreshold = currentTime - _settings.DeliveryExpiration;

        Expression<Func<OutboxEntry, bool>> canBeClaimed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.DeliveryAttempts < _settings.MaxDeliveryCount) ||
            (entry.State == EntryState.InProcessing && entry.DeliveryAttempts < _settings.MaxDeliveryCount && entry.Updated < validProcessingThreshold);

        Guid[] entriesToClaimIds = await _dbContext
            .Set<OutboxEntry>()
            .Where(canBeClaimed)
            .OrderBy(entry => entry.Created)
            .Select(entry => entry.Id)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        if (entriesToClaimIds.Length == 0)
        {
            return [];
        }

        Guid claimId = Guid.NewGuid();

        await _dbContext
            .Set<OutboxEntry>()
            .Where(canBeClaimed)
            .Where(entry => entriesToClaimIds.Contains(entry.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.DeliveryAttempts, entry => entry.DeliveryAttempts + 1)
                .SetProperty(entry => entry.ClaimId, claimId), cancellationToken)
            .ConfigureAwait(false);

        return
        [
            ..await _dbContext
                .Set<OutboxEntry>()
                .Where(entry => entry.ClaimId == claimId)
                .OrderBy(entry => entry.Created)
                .Select(entry => new SerializedRoutedEvent(
                    entry.Id,
                    EventIdentity.Parse(entry.Event),
                    EventHandlerIdentity.Parse(entry.Handler),
                    Route.Parse(entry.Route),
                    entry.Payload))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false)
        ];
    }

    public async Task MarkAsDelivered(Guid entryId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        await _dbContext
            .Set<OutboxEntry>()
            .Where(entry => 
                entry.Id == entryId && 
                entry.State == EntryState.InProcessing)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.Processed)
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.ClaimId, (Guid?)null), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        await _dbContext
            .Set<OutboxEntry>()
            .Where(entry => 
                entry.Id == entryId && 
                entry.State == EntryState.InProcessing)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.Failed)
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.ClaimId, (Guid?)null), cancellationToken)
            .ConfigureAwait(false);
    }
}
