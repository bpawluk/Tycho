using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxConsumer(
    IEventSerializer eventSerializer,
    TychoDbContext dbContext,
    InboxConsumerSettings? settings = null) : IInboxConsumer
{
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly InboxConsumerSettings _settings = settings ?? InboxConsumerSettings.Default;

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
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Updated)
            .ThenBy(entry => entry.Id)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, utcNow)
                .SetProperty(entry => entry.State, EntryState.InProcessing)
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
            .SingleOrDefaultAsync(entry => entry.ClaimId == claimId, cancellationToken)
            .ConfigureAwait(false);

        if (entryToDeliver == null)
        {
            return null;
        }

        var serializedEvent = new SerializedRoutedEvent(
            entryToDeliver.Id,
            entryToDeliver.PublishId,
            EventIdentity.Parse(entryToDeliver.Event),
            EventHandlerIdentity.Parse(entryToDeliver.Handler),
            Route.Empty(),
            entryToDeliver.Payload);

        RoutedEvent? routedEvent = await TryDeserializeWith(_eventSerializer, serializedEvent, claimId, cancellationToken).ConfigureAwait(false);
        return routedEvent == null ? null : new InboxEvent(claimId, routedEvent);
    }

    public async Task<bool> MarkAsHandledAsync(Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRowsCount = await _dbContext
            .Set<InboxEntry>()
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
            .Set<InboxEntry>()
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

    private async Task<RoutedEvent?> TryDeserializeWith(
        IEventSerializer eventSerializer,
        SerializedRoutedEvent serializedEvent,
        Guid claimId,
        CancellationToken cancellationToken)
    {
        try
        {
            return eventSerializer.Deserialize(serializedEvent);
        }
        catch
        {
            await MarkAsFailedAsync(claimId, cancellationToken).ConfigureAwait(false);
            return null;
        }
    }
}
