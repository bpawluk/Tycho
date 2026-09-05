using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal class EmptyTransaction : ITransaction
    {
        public bool IsInProgress => false;

        public void ExecuteAfterCommit(Action action) { }

        public Task BeginAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("No transaction provider is configured.");
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("No transaction provider is configured.");
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("No transaction provider is configured.");
        }

        public ValueTask DisposeAsync() => default;

        public void Dispose() { }
    }
}
