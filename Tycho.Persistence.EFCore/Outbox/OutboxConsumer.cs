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

    public async Task<IReadOnlyCollection<OutboxEvent>> Read(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return [];
        }

        DateTime claimCheckTime = DateTime.UtcNow;
        Expression<Func<OutboxEntry, bool>> canBeProcessed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.DeliveryAttempts < _settings.MaxDeliveryCount) ||
            (entry.State == EntryState.InProcessing && entry.DeliveryAttempts < _settings.MaxDeliveryCount && entry.ClaimExpiration < claimCheckTime);

        Guid[] entriesToClaimIds = await _dbContext
            .Set<OutboxEntry>()
            .Where(canBeProcessed)
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
        DateTime claimWriteTime = DateTime.UtcNow;

        // Known flaw: relying on system clock for claiming is vulnerable to clock skew when running the app on multiple machines
        // Recommendation: set DeliveryExpiration to values significantly higher than the potential clock skew to mitigate this issue
        await _dbContext
            .Set<OutboxEntry>()
            .Where(canBeProcessed)
            .Where(entry => entriesToClaimIds.Contains(entry.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, claimWriteTime)
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.DeliveryAttempts, entry => entry.DeliveryAttempts + 1)
                .SetProperty(entry => entry.ClaimId, claimId)
                .SetProperty(entry => entry.ClaimExpiration, claimWriteTime + _settings.DeliveryExpiration), cancellationToken)
            .ConfigureAwait(false);

        return
        [
            ..await _dbContext
                .Set<OutboxEntry>()
                .Where(entry => entry.ClaimId == claimId)
                .OrderBy(entry => entry.Created)
                .Select(entry => new OutboxEvent(
                    claimId,
                    new SerializedRoutedEvent(
                        entry.Id,
                        EventIdentity.Parse(entry.Event),
                        EventHandlerIdentity.Parse(entry.Handler),
                        Route.Parse(entry.Route),
                        entry.Payload)))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false)
        ];
    }

    public async Task<bool> MarkAsDelivered(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRows = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.Id == entryId &&
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.State, EntryState.Processed)
                .SetProperty(entry => entry.ClaimId, Guid.Empty)
                .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue), cancellationToken)
            .ConfigureAwait(false);

        return updatedRows == 1;
    }

    public async Task<bool> MarkAsFailed(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRows = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.Id == entryId &&
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.State, EntryState.Failed)
                .SetProperty(entry => entry.ClaimId, Guid.Empty)
                .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue), cancellationToken)
            .ConfigureAwait(false);

        return updatedRows == 1;
    }
}
