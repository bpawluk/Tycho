using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal interface ITransaction
    {
        bool IsInProgress { get; }

        void ExecuteAfterCommit(Action action);

        Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);

        Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);
    }
}
