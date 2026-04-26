using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Events.Serialization;
using Tycho.Structure;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(Internals internals, OutboxConsumerSettings? settings = null) : IOutboxConsumer
{
    private readonly Internals _internals = internals;
    private readonly OutboxConsumerSettings _settings = settings ?? OutboxConsumerSettings.Default;

    // TODO: concurrency handling
    // TODO: dead letter handling
    public async Task<IReadOnlyCollection<RoutedEvent>> Read(int count, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();
        var eventSerializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        var currentTime = DateTime.UtcNow;
        var validProcessingThreshold = currentTime - _settings.InDeliveryStateExpiration;

        var entriesToDeliver = await dbContext
            .Set<OutboxEntry>()
            .Where(entry =>
                (entry.State == EntryState.New) ||
                (entry.State == EntryState.Failed && entry.DeliveryAttempts < _settings.MaxDeliveryCount) ||
                (entry.State == EntryState.InDelivery && entry.DeliveryAttempts < _settings.MaxDeliveryCount && entry.Updated < validProcessingThreshold))
            .OrderBy(entry => entry.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var entry in entriesToDeliver)
        {
            entry.State = EntryState.InDelivery;
            entry.Updated = currentTime;
            entry.DeliveryAttempts++;
        }
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return
        [
            ..entriesToDeliver
                .Select(entry => new SerializedEvent(entry.Id, entry.Event, entry.Handler, entry.Route, entry.Payload))
                .Select(eventSerializer.Deserialize)
        ];
    }

    public async Task MarkAsDelivered(Guid entryId, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var outboxMessages = dbContext.Set<OutboxEntry>();
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

        var outboxMessages = dbContext.Set<OutboxEntry>();
        var entry = await outboxMessages.FindAsync([entryId], cancellationToken).ConfigureAwait(false);

        if (entry != null)
        {
            entry.State = EntryState.Failed;
            entry.Updated = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}