using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Outbox;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(TychoDbContext dbContext, OutboxActivity outboxActivity) : IOutboxWriter
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxActivity _outboxActivity = outboxActivity;

    public async Task Write(IReadOnlyCollection<OutboxEntry> entries, bool shouldCommit, CancellationToken cancellationToken)
    {
        foreach (var entry in entries)
        {
            var outboxMessage = new OutboxMessage // TODO
            {
                Id = entry.Id,
                Payload = (entry.Payload as string)!,
            };
            _dbContext.Set<OutboxMessage>().Add(outboxMessage);
        }

        if (shouldCommit)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _outboxActivity.NotifyNewEntriesAdded();
    }

    public Task Write(IReadOnlyCollection<OutboxEntry> entries, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}