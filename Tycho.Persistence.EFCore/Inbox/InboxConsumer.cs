using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Entries;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Logging;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxConsumer(
    IEventSerializer eventSerializer,
    TychoDbContext dbContext,
    PersistenceOwner owner,
    InboxConsumerSettings settings,
    ILogger<InboxConsumer>? logger = null) : IInboxConsumer
{
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly InboxConsumerSettings _settings = settings;
    private readonly ILogger<InboxConsumer> _logger = logger ?? NullLogger<InboxConsumer>.Instance;

    public async Task<InboxEvent?> TryReadAsync(CancellationToken cancellationToken)
    {
        Guid claimId = Guid.NewGuid();
        DateTime utcNow = DateTime.UtcNow;

        var claimRequest = new ClaimRequest(claimId, utcNow, utcNow + _settings.ProcessingExpiration, _settings.MaxProcessingCount);
        ClaimResult<InboxEntry> result = await ClaimInboxEntryCommand.ExecuteAsync(dbContext, owner, claimRequest, cancellationToken).ConfigureAwait(false);
        if (result.Status == ClaimStatus.NotClaimed)
            return null;

        InboxEntry? entryToDeliver = result.Entry;

        if (entryToDeliver == null)
        {
            _logger.InboxClaimedEntryMissing(claimId);
            return null;
        }

        try
        {
            var serializedEvent = new SerializedEvent(
                entryToDeliver.EntryId,
                entryToDeliver.PublishId,
                EventIdentity.Parse(entryToDeliver.Event),
                EventHandlerIdentity.Parse(entryToDeliver.Handler),
                entryToDeliver.Payload);

            Event deserializedEvent = _eventSerializer.Deserialize(serializedEvent);
            return new InboxEvent(claimId, deserializedEvent);
        }
        catch (Exception exception)
        {
            try
            {
                bool markedAsFailed = await MarkAsFailedAsync(entryToDeliver.EntryId, claimId, cancellationToken).ConfigureAwait(false);
                if (!markedAsFailed)
                {
                    logger?.InboxMessageStatusUpdateFailed(entryToDeliver.EntryId, claimId);
                }
            }
            catch (Exception failureException)
            {
                throw new AggregateException("Failed to deserialize an inbox entry and mark it as failed.", exception, failureException);
            }
            throw;
        }
    }

    public async Task<bool> MarkAsHandledAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        return await CompleteInboxClaimCommand.ExecuteAsync(dbContext, owner, entryId, claimId, DateTime.UtcNow, cancellationToken).ConfigureAwait(false) == 1;
    }

    public async Task<bool> MarkAsFailedAsync(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        return await FailInboxClaimCommand.ExecuteAsync(dbContext, owner, entryId, claimId, DateTime.UtcNow, cancellationToken).ConfigureAwait(false) == 1;
    }

}
