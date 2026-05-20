using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal interface ITransaction : IAsyncDisposable, IDisposable
    {
        bool IsInProgress { get; }

        void ExecuteAfterCommit(Action action);

        Task BeginAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
