using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.Transactions;

public abstract class TransactionalRequestHandler<TRequest>(TychoDbContext dbContext)
    : ITransactionalRequestHandler<TRequest>
    where TRequest : class, IRequest
{
    private readonly TychoDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    protected virtual IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

    public abstract Task HandleAsync(TRequest requestData, CancellationToken cancellationToken);

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

public abstract class TransactionalRequestHandler<TRequest, TResponse>(TychoDbContext dbContext)
    : ITransactionalRequestHandler<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly TychoDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    protected virtual IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

    public abstract Task<TResponse> HandleAsync(TRequest requestData, CancellationToken cancellationToken);

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
