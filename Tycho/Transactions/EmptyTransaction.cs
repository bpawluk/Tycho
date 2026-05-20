using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal class EmptyTransaction : ITransaction
    {
        public bool IsInProgress => false;

        public void ExecuteAfterCommit(Action action) { }

        public Task BeginAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task CommitAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task RollbackAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public ValueTask DisposeAsync() => default;

        public void Dispose() { }
    }
}
