using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// Base interface for request handlers wrapping their logic in transactions.
    /// </summary>
    public interface ITransactionalRequestHandler : IRequestHandler
    {
        /// <summary>
        /// Begins a transaction that will wrap request handling operations.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Transactional request handler for a request of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
    public interface ITransactionalRequestHandler<TRequest> : ITransactionalRequestHandler, IRequestHandler<TRequest>
        where TRequest : class, IRequest
    {
    }

    /// <summary>
    /// Transactional request handler for a request of type <typeparamref name="TRequest"/> with response
    /// <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
    /// <typeparam name="TResponse">The type of the response to return.</typeparam>
    public interface ITransactionalRequestHandler<TRequest, TResponse> : ITransactionalRequestHandler, IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
    }
}
