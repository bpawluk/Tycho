using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Transactions;

internal sealed class Transaction(TychoDbContext dbContext) : ITransaction
{
    private readonly TychoDbContext _dbContext = dbContext;
    private IDbContextTransaction? _activeTransaction;

    public bool IsInProgress => _activeTransaction is not null;

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is not null)
        {
            return;
        }

        _activeTransaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is null)
        {
            return;
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _activeTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        await DisposeActiveTransactionAsync().ConfigureAwait(false);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is null)
        {
            return;
        }

        try
        {
            await _activeTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await DisposeActiveTransactionAsync().ConfigureAwait(false);
        }
    }

    private async Task DisposeActiveTransactionAsync()
    {
        IDbContextTransaction? activeTransaction = _activeTransaction;
        _activeTransaction = null;

        if (activeTransaction is not null)
        {
            await activeTransaction.DisposeAsync().ConfigureAwait(false);
        }
    }
}
