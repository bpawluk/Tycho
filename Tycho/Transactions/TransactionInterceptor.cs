using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;

namespace Tycho.Transactions
{
    internal sealed class TransactionInterceptor<TRequest, TResponse> : IRequestInterceptor<TRequest, TResponse>
        where TRequest : class
    {
        private readonly ITransaction _transaction;

        public TransactionInterceptor(ITransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task<TResponse> InterceptAsync(
            RequestHandlerDelegate<TRequest, TResponse> next,
            TRequest requestData,
            CancellationToken cancellationToken)
        {
            await _transaction.BeginAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                TResponse response = await next(requestData, cancellationToken).ConfigureAwait(false);
                if (_transaction.IsInProgress)
                {
                    await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                return response;
            }
            catch
            {
                if (_transaction.IsInProgress)
                {
                    await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}
