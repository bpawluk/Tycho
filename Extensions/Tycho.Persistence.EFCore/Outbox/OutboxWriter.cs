using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxWriter(TychoDbContext dbContext, OutboxActivity outboxActivity) : IOutboxWriter
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly OutboxActivity _outboxActivity = outboxActivity;

    public Task Write(IReadOnlyCollection<OutboxEntry> entries, bool shouldCommit, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}