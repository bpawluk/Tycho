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
    public interface ITransactionalRequestHandler<TRequest> : ITransactionalRequestHandler
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Handles a request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="requestData">The data of the request to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task Handle(TRequest requestData, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Transactional request handler for a request of type <typeparamref name="TRequest"/> with response
    /// <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to handle.</typeparam>
    /// <typeparam name="TResponse">The type of the response to return.</typeparam>
    public interface ITransactionalRequestHandler<TRequest, TResponse> : ITransactionalRequestHandler
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Handles a request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="requestData">The data of the request to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A response of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken);
    }
}
