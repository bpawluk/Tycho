using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Model;
using Tycho.Events.Inbox;
using Tycho.Events.Routing;
using Tycho.Identity.Events;
using Tycho.Persistence.EFCore.Common;
using Tycho.Structure;
using Tycho.Events.Serialization;

namespace Tycho.Persistence.EFCore.Inbox;

internal class InboxConsumer(Internals internals, InboxConsumerSettings? settings = null) : IInboxConsumer
{
    private readonly Internals _internals = internals;
    private readonly InboxConsumerSettings _settings = settings ?? InboxConsumerSettings.Default;

    // TODO: concurrency handling

    // TODO: dead letter handling

    public async Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();
        var eventSerializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        var currentTime = DateTime.UtcNow;
        var validProcessingThreshold = currentTime - _settings.ProcessingExpiration;

        var entriesToDeliver = await dbContext
            .Set<InboxEntry>()
            .Where(entry =>
                (entry.State == EntryState.New) ||
                (entry.State == EntryState.Failed && entry.ProcessingAttempts < _settings.MaxProcessingCount) ||
                (entry.State == EntryState.InProcessing && entry.ProcessingAttempts < _settings.MaxProcessingCount && entry.Updated < validProcessingThreshold))
            .OrderBy(entry => entry.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var entry in entriesToDeliver)
        {
            entry.State = EntryState.InProcessing;
            entry.Updated = currentTime;
            entry.ProcessingAttempts++;
        }
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var result = new List<RoutedEvent>();
        foreach (var entry in entriesToDeliver)
        {
            var serializedEvent = new SerializedRoutedEvent(
                entry.Id,
                EventIdentity.Parse(entry.Event),
                EventHandlerIdentity.Parse(entry.Handler),
                Route.Empty(),
                entry.Payload);

            var routedEvent = await DeserializeWith(eventSerializer, serializedEvent).ConfigureAwait(false);
            if (routedEvent is not null)
            {
                result.Add(routedEvent);
            }
        }
        return result;
    }

    public async Task MarkAsHandled(Guid entryId, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var outboxMessages = dbContext.Set<InboxEntry>();
        var entry = await outboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            outboxMessages.Remove(entry);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task MarkAsFailed(Guid entryId, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var outboxMessages = dbContext.Set<InboxEntry>();
        var entry = await outboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            entry.State = EntryState.Failed;
            entry.Updated = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<RoutedEvent?> DeserializeWith(IEventSerializer eventSerializer, SerializedRoutedEvent serializedEvent)
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