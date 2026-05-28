using System;
using System.Collections.Generic;
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

    public async Task<IReadOnlyCollection<InboxEvent>> Read(int count, CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return [];
        }

        DateTime claimCheckTime = DateTime.UtcNow;
        Expression<Func<InboxEntry, bool>> canBeProcessed = entry =>
            (entry.State == EntryState.New) ||
            (entry.State == EntryState.Failed && entry.ProcessingAttempts < _settings.MaxProcessingCount) ||
            (entry.State == EntryState.InProcessing && entry.ProcessingAttempts < _settings.MaxProcessingCount && entry.ClaimExpiration < claimCheckTime);

        Guid[] entriesToClaimIds = await _dbContext
            .Set<InboxEntry>()
            .Where(canBeProcessed)
            .OrderBy(entry => entry.Created)
            .Select(entry => entry.Id)
            .Take(count)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);

        if (entriesToClaimIds.Length == 0)
        {
            return [];
        }

        Guid claimId = Guid.NewGuid();
        DateTime claimWriteTime = DateTime.UtcNow;

        // Known flaw: relying on system clock for claiming is vulnerable to clock skew when running the app on multiple machines
        // Recommendation: set ProcessingExpiration to values significantly higher than the potential clock skew to mitigate this issue

        await _dbContext
            .Set<InboxEntry>()
            .Where(canBeProcessed)
            .Where(entry => entriesToClaimIds.Contains(entry.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(entry => entry.Updated, claimWriteTime)
                .SetProperty(entry => entry.State, EntryState.InProcessing)
                .SetProperty(entry => entry.ProcessingAttempts, entry => entry.ProcessingAttempts + 1)
                .SetProperty(entry => entry.ClaimId, claimId)
                .SetProperty(entry => entry.ClaimExpiration, claimWriteTime + _settings.ProcessingExpiration), cancellationToken)
            .ConfigureAwait(false);

        InboxEntry[] entriesToDeliver = await _dbContext
            .Set<InboxEntry>()
            .Where(entry => entry.ClaimId == claimId)
            .OrderBy(entry => entry.Created)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var result = new List<InboxEvent>();
        foreach (InboxEntry entry in entriesToDeliver)
        {
            var serializedEvent = new SerializedRoutedEvent(
                entry.Id,
                EventIdentity.Parse(entry.Event),
                EventHandlerIdentity.Parse(entry.Handler),
                Route.Empty(),
                entry.Payload);

            RoutedEvent? routedEvent = await TryDeserializeWith(_eventSerializer, serializedEvent, claimId, cancellationToken).ConfigureAwait(false);
            if (routedEvent is not null)
            {
                result.Add(new InboxEvent(claimId, routedEvent));
            }
        }

        return result;
    }

    public async Task<bool> MarkAsHandled(Guid entryId, Guid claimId, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        int updatedRows = await _dbContext
            .Set<InboxEntry>()
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
            .Set<InboxEntry>()
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
            await MarkAsFailed(serializedEvent.Id, claimId, cancellationToken).ConfigureAwait(false);
            return null;
        }
    }
}
