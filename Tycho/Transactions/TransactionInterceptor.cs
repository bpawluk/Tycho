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

        public Task<TResponse> InterceptAsync(
            RequestHandlerDelegate<TRequest, TResponse> next,
            TRequest requestData,
            CancellationToken cancellationToken)
        {
            return _transaction.ExecuteAsync(
                token => next(requestData, token),
                cancellationToken);
        }
    }
}
