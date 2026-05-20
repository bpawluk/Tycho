using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Transactions;

internal sealed class Transaction(TychoDbContext dbContext) : ITransaction
{
    private readonly TychoDbContext _dbContext = dbContext;
    private readonly List<Action> _afterCommitActions = [];
    private IDbContextTransaction? _activeTransaction;

    public bool IsInProgress { get; private set; }

    public void ExecuteAfterCommit(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _afterCommitActions.Add(action);
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is not null)
        {
            return;
        }

        _activeTransaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        IsInProgress = true;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is null)
        {
            return;
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _activeTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        IsInProgress = false;

        foreach (Action afterCommitAction in _afterCommitActions)
        {
            afterCommitAction();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_activeTransaction is null)
        {
            return;
        }

        await _activeTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        IsInProgress = false;
    }

    public async ValueTask DisposeAsync()
    {
        _afterCommitActions.Clear();
        if (_activeTransaction is not null)
        {
            await _activeTransaction.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _afterCommitActions.Clear();
        _activeTransaction?.Dispose();
    }
}
