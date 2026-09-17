using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Tycho.Transactions;

namespace Tycho.Persistence.EFCore.Transactions;

internal sealed class Transaction(TychoDbContext dbContext, ILogger<Transaction>? logger = null) : ITransaction
{
    private const int NotStarted = 0;
    private const int InProgress = 1;
    private const int Finished = 2;

    private readonly TychoDbContext _dbContext = dbContext;
    private int _executionState;

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        async Task<bool> ExecuteWithStubResult(CancellationToken token)
        {
            await operation(token).ConfigureAwait(false);
            return true;
        }
        await ExecuteAsync(ExecuteWithStubResult, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (ExecutionStrategy.Current?.RetriesOnFailure == true)
        {
            throw new InvalidOperationException("Transactions managed by Tycho cannot execute inside an active retrying EF Core execution strategy.");
        }

        if (Interlocked.CompareExchange(ref _executionState, InProgress, NotStarted) != NotStarted)
        {
            throw new InvalidOperationException("This transaction has already been executed or is currently in progress.");
        }

        TResult result;
        try
        {
            var executionStrategy = new NonRetryingScopeExecutionStrategy(_dbContext);
            result = await executionStrategy.ExecuteAsync(token => ExecuteTransactionAsync(operation, token), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            Volatile.Write(ref _executionState, Finished);
        }

        return result;
    }

    private async Task<TResult> ExecuteTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
    {
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            TResult result = await operation(cancellationToken).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            await transaction.CommitAsync(CancellationToken.None).ConfigureAwait(false);

            return result;
        }
        catch
        {
            try
            {
                await transaction.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger?.LogError(exception, "Failed to roll back the transaction.");
            }
            finally
            {
                _dbContext.ChangeTracker.Clear();
            }
            throw;
        }
        finally
        {
            try
            {
                await transaction.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger?.LogError(exception, "Failed to dispose the transaction.");
            }
        }
    }
}
