using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Identity.Events;
using Tycho.Identity.Structure;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Logging;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(
    TychoDbContext dbContext,
    PersistenceOwner owner,
    OutboxConsumerSettings settings,
    ILogger<OutboxConsumer>? logger = null) : IOutboxConsumer
{
    private readonly OutboxConsumerSettings _settings = settings;
    private readonly ILogger<OutboxConsumer> _logger = logger ?? NullLogger<OutboxConsumer>.Instance;

    public async Task<OutboxEvent?> TryReadAsync(CancellationToken cancellationToken)
    {
        Guid claimId = Guid.NewGuid();
        DateTime utcNow = DateTime.UtcNow;

        var claimRequest = new ClaimRequest(claimId, utcNow, utcNow + _settings.DeliveryExpiration, _settings.MaxDeliveryCount);
        ClaimResult<OutboxEntry> result = await ClaimOutboxEntryCommand
            .ExecuteAsync(dbContext, owner, claimRequest, cancellationToken)
            .ConfigureAwait(false);

        if (result.Status == ClaimStatus.NotClaimed)
        {
            return null;
        }

        OutboxEntry? entryToDeliver = result.Entry;
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

    public async Task<bool> MarkAsDeliveredAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        return await CompleteOutboxClaimCommand
            .ExecuteAsync(dbContext, owner, entryId, claimId, DateTime.UtcNow, cancellationToken)
            .ConfigureAwait(false) == 1;
    }

    public async Task<bool> MarkAsFailedAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        return await FailOutboxClaimCommand
            .ExecuteAsync(dbContext, owner, entryId, claimId, DateTime.UtcNow, cancellationToken)
            .ConfigureAwait(false) == 1;
    }
}
