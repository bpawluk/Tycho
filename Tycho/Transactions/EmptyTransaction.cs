using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Transactions
{
    internal class EmptyTransaction : ITransaction
    {
        public bool IsInProgress => false;

        public Task BeginAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
