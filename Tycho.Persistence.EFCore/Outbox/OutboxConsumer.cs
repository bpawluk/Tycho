using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Logging;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(
    TychoDbContext dbContext,
    PersistenceOwner owner,
    OutboxConsumerSettings settings,
    ILogger<OutboxConsumer>? logger = null) : IOutboxConsumer
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly PersistenceOwner _owner = owner;
    private readonly OutboxConsumerSettings _settings = settings;
    private readonly ILogger<OutboxConsumer> _logger = logger ?? NullLogger<OutboxConsumer>.Instance;

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
            .Where(entry => entry.OwnerId == _owner.Identifier)
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Updated)
            .ThenBy(entry => entry.EntryId)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.Updated, utcNow)
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
            .SingleOrDefaultAsync(entry =>
                entry.OwnerId == _owner.Identifier &&
                entry.ClaimId == claimId, cancellationToken)
            .ConfigureAwait(false);

        if (entryToDeliver == null)
        {
            _logger.OutboxClaimedEntryMissing(claimId);
            return null;
        }

        return new OutboxEvent(
            claimId,
            new SerializedRoutedEvent(
                entryToDeliver.EntryId,
                entryToDeliver.PublishId,
                EventIdentity.Parse(entryToDeliver.Event),
                EventHandlerIdentity.Parse(entryToDeliver.Handler),
                InstanceIdentity.Parse(entryToDeliver.Destination),
                entryToDeliver.Payload));
    }

    public async Task<bool> MarkAsDeliveredAsync(Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRowsCount = await _dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                entry.OwnerId == _owner.Identifier &&
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.Processed)
                .SetProperty(entry => entry.Updated, currentTime)
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
                entry.OwnerId == _owner.Identifier &&
                entry.State == EntryState.InProcessing &&
                entry.ClaimId == claimId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.Failed)
                .SetProperty(entry => entry.Updated, currentTime)
                .SetProperty(entry => entry.ClaimId, Guid.Empty)
                .SetProperty(entry => entry.ClaimExpiration, DateTime.MinValue), cancellationToken)
            .ConfigureAwait(false);

        return updatedRowsCount == 1;
    }
}
