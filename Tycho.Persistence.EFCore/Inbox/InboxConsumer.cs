using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;
using Tycho.Persistence.EFCore.Logging;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxConsumer(
    IEventSerializer eventSerializer,
    TychoDbContext dbContext,
    PersistenceOwner owner,
    InboxConsumerSettings settings,
    ILogger<InboxConsumer>? logger = null) : IInboxConsumer
{
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly PersistenceOwner _owner = owner;
    private readonly InboxConsumerSettings _settings = settings;
    private readonly ILogger<InboxConsumer> _logger = logger ?? NullLogger<InboxConsumer>.Instance;

    public async Task<InboxEvent?> TryReadAsync(CancellationToken cancellationToken)
    {
        Guid claimId = Guid.NewGuid();
        DateTime utcNow = DateTime.UtcNow;

        Expression<Func<InboxEntry, bool>> canBeProcessed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.ProcessingAttempts < _settings.MaxProcessingCount) ||
            (entry.State == EntryState.InProcessing && entry.ProcessingAttempts < _settings.MaxProcessingCount && entry.ClaimExpiration < utcNow);

        int claimedEntriesCount = await _dbContext
            .Set<InboxEntry>()
            .Where(entry => entry.OwnerId == _owner.Identifier)
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Updated)
            .ThenBy(entry => entry.EntryId)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.Updated, utcNow)
                .SetProperty(entry => entry.ProcessingAttempts, entry => entry.ProcessingAttempts + 1)
                .SetProperty(entry => entry.ClaimId, claimId)
                .SetProperty(entry => entry.ClaimExpiration, utcNow + _settings.ProcessingExpiration), cancellationToken)
            .ConfigureAwait(false);

        if (claimedEntriesCount != 1)
        {
            return null;
        }

        InboxEntry? entryToDeliver = await _dbContext
            .Set<InboxEntry>()
            .AsNoTracking()
            .SingleOrDefaultAsync(entry =>
                entry.OwnerId == _owner.Identifier &&
                entry.ClaimId == claimId, cancellationToken)
            .ConfigureAwait(false);

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
                bool markedAsFailed = await MarkAsFailedAsync(claimId, cancellationToken).ConfigureAwait(false);
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

    public async Task<bool> MarkAsHandledAsync(Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRowsCount = await _dbContext
            .Set<InboxEntry>()
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
            .Set<InboxEntry>()
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
