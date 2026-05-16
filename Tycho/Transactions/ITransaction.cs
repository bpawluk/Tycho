using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal interface ITransaction
    {
        bool IsInProgress { get; }

        Task BeginAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
