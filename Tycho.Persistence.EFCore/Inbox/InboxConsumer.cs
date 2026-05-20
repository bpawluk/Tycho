using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Inbox;
using Tycho.Events.Model;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxConsumer(
    ITransaction transaction,
    IEventSerializer eventSerializer,
    TychoDbContext dbContext,
    InboxConsumerSettings? settings = null) : IInboxConsumer
{
    private readonly ITransaction _transaction = transaction;
    private readonly IEventSerializer _eventSerializer = eventSerializer;
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly InboxConsumerSettings _settings = settings ?? InboxConsumerSettings.Default;

    public async Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;
        DateTime validProcessingThreshold = currentTime - _settings.ProcessingExpiration;

        InboxEntry[] entriesToDeliver = await _dbContext
            .Set<InboxEntry>()
            .Where(entry =>
                (entry.State == EntryState.New) ||
                (entry.State == EntryState.Failed && entry.ProcessingAttempts < _settings.MaxProcessingCount) ||
                (entry.State == EntryState.InProcessing && entry.ProcessingAttempts < _settings.MaxProcessingCount && entry.Updated < validProcessingThreshold))
            .OrderBy(entry => entry.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (InboxEntry? entry in entriesToDeliver)
        {
            entry.State = EntryState.InProcessing;
            entry.Updated = currentTime;
            entry.ProcessingAttempts++;
        }

        if (!_transaction.IsInProgress)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        var result = new List<RoutedEvent>();
        foreach (InboxEntry? entry in entriesToDeliver)
        {
            var serializedEvent = new SerializedRoutedEvent(
                entry.Id,
                EventIdentity.Parse(entry.Event),
                EventHandlerIdentity.Parse(entry.Handler),
                Route.Empty(),
                entry.Payload);

            RoutedEvent? routedEvent = await TryDeserializeWith(_eventSerializer, serializedEvent).ConfigureAwait(false);
            if (routedEvent is not null)
            {
                result.Add(routedEvent);
            }
        }
        return result;
    }

    public async Task MarkAsHandled(Guid entryId, CancellationToken cancellationToken)
    {
        DbSet<InboxEntry> inboxMessages = _dbContext.Set<InboxEntry>();
        InboxEntry? entry = await inboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

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
        DbSet<InboxEntry> inboxMessages = _dbContext.Set<InboxEntry>();
        InboxEntry? entry = await inboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

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

    private async Task<RoutedEvent?> TryDeserializeWith(IEventSerializer eventSerializer, SerializedRoutedEvent serializedEvent)
    {
        try
        {
            return eventSerializer.Deserialize(serializedEvent);
        }
        catch
        {
            await MarkAsFailed(serializedEvent.Id, CancellationToken.None).ConfigureAwait(false);
            return null;
        }
    }
}
