using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycho.Events.Routing;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(TychoDbContext dbContext, OutboxConsumerSettings? settings = null) : IOutboxConsumer
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxConsumerSettings _settings = settings ?? OutboxConsumerSettings.Default;

    public async Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
    {
        var currentTime = DateTime.UtcNow;
        var validProcessingThreshold = currentTime - _settings.ProcessingStateExpiration;
        var outboxMessages = _dbContext.Set<OutboxMessage>();

        var messagesToProcess = await outboxMessages
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
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return messagesToProcess
            .Select(message => new OutboxEntry(
                message.Id,
                HandlerIdentity.FromString(message.Handler), 
                message.Payload))
            .ToArray();
    }

    public async Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        var outboxMessages = _dbContext.Set<OutboxMessage>();
        var message = await outboxMessages.FindAsync([entry.Id], cancellationToken).ConfigureAwait(false);
        if (message != null)
        {
            outboxMessages.Remove(message);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        var outboxMessages = _dbContext.Set<OutboxMessage>();
        var message = await outboxMessages.FindAsync([entry.Id], cancellationToken).ConfigureAwait(false);
        if (message != null)
        {
            message.State = MessageState.Failed;
            message.Updated = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}