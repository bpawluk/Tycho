using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal class EmptyTransaction : ITransaction
    {
        public bool IsInProgress => false;

        public void ExecuteAfterCommit(Action action) { }

        public Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("No transaction provider is configured.");
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("No transaction provider is configured.");
        }
    }
}
