using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxConsumer(TychoDbContext dbContext) : IOutboxConsumer
{
    private readonly TychoDbContext _dbContext = dbContext;

    public Task<IReadOnlyCollection<OutboxEntry>> Read(int count, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task MarkAsProcessed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task MarkAsFailed(OutboxEntry entry, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}