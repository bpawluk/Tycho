using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(TychoDbContext dbContext, OutboxActivity outboxActivity) : IOutboxWriter
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxActivity _outboxActivity = outboxActivity;

    public async Task Write(IReadOnlyCollection<OutboxEntry> entries, bool shouldCommit, CancellationToken cancellationToken)
    {
        var outboxMessages = _dbContext.Set<OutboxMessage>();

        foreach (var entry in entries)
        {
            var outboxMessage = new OutboxMessage
            {
                Id = entry.Id,
                Handler = entry.HandlerIdentity.ToString(),
                Payload = (entry.Payload as string)!,
            };
            outboxMessages.Add(outboxMessage);
        }

        if (shouldCommit)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _outboxActivity.NotifyNewEntriesAdded();
    }
}