using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Events;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Transactions;

public abstract class TransactionalEventHandler<TEvent>(TychoDbContext dbContext) : ITransactionalEventHandler<TEvent>
    where TEvent : class, IEvent
{
    private readonly TychoDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    protected virtual IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

    public abstract Task HandleAsync(EventContext<TEvent> context, CancellationToken cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel, cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await _transaction!.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        await _transaction!.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }
}
