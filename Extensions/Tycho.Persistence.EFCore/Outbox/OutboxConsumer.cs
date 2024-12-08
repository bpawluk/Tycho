using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Structure.Internal;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(Internals internals, OutboxConsumerSettings? settings = null) : IOutboxConsumer
{
    private readonly Internals _internals = internals;
    private readonly OutboxConsumerSettings _settings = settings ?? OutboxConsumerSettings.Default;

    public async Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var currentTime = DateTime.UtcNow;
        var validProcessingThreshold = currentTime - _settings.ProcessingStateExpiration;

        var messagesToProcess = await dbContext
            .Set<OutboxMessage>()
            .Where(message =>
                (message.State == MessageState.New) ||
                (message.State == MessageState.Failed &&
                 message.DeliveryCount < _settings.MaxDeliveryCount) ||
                (message.State == MessageState.Processing &&
                 message.DeliveryCount < _settings.MaxDeliveryCount &&
                 message.Updated < validProcessingThreshold))
            .OrderBy(message => message.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (var message in messagesToProcess)
        {
            message.State = MessageState.Processing;
            message.Updated = currentTime;
            message.DeliveryCount++;
        }
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return messagesToProcess
            .Select(message => new OutboxEntry(
                message.Id,
                HandlerIdentity.FromString(message.Handler), 
                message.Payload))
            .ToArray();
    }

    public async Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var outboxMessages = dbContext.Set<OutboxMessage>();
        var message = await outboxMessages.FindAsync([entry.Id], cancellationToken).ConfigureAwait(false);

        if (message != null)
        {
            outboxMessages.Remove(message);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        using var scope = _internals.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<TychoDbContext>();

        var outboxMessages = dbContext.Set<OutboxMessage>();
        var message = await outboxMessages.FindAsync([entry.Id], cancellationToken).ConfigureAwait(false);

        if (message != null)
        {
            message.State = MessageState.Failed;
            message.Updated = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}