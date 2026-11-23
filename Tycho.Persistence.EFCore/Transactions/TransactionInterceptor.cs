using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Tycho.Persistence.EFCore.Transactions;

internal sealed class TransactionInterceptor : DbTransactionInterceptor
{
    private readonly ConcurrentDictionary<Guid, Action> _onCommittedActions = new();

    public void ExecuteAfterCommit(Guid transactionId, Action callback) => _onCommittedActions.TryAdd(transactionId, callback);

    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        NotifyAfterCommit(eventData.TransactionId);
    }

    public override Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken)
    {
        NotifyAfterCommit(eventData.TransactionId);
        return Task.CompletedTask;
    }

    public override void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
    {
        _onCommittedActions.TryRemove(eventData.TransactionId, out _);
    }

    public override Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken)
    {
        _onCommittedActions.TryRemove(eventData.TransactionId, out _);
        return Task.CompletedTask;
    }

    public override void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
    {
        _onCommittedActions.TryRemove(eventData.TransactionId, out _);
    }

    public override Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken)
    {
        _onCommittedActions.TryRemove(eventData.TransactionId, out _);
        return Task.CompletedTask;
    }

    private void NotifyAfterCommit(Guid transactionId)
    {
        if (_onCommittedActions.TryRemove(transactionId, out Action? commitCallback))
        {
            commitCallback();
        }
    }
}
