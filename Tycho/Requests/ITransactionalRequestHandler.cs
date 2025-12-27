using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// TBD
    /// </summary>
    public interface ITransactionalRequestHandler : IRequestHandler
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// TBD
    /// </summary>
    public interface ITransactionalRequestHandler<TRequest> : ITransactionalRequestHandler
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Handles a request of type <typeparamref name="TRequest"/>
        /// </summary>
        /// <param name="requestData">The data of the event to handle</param>
        Task Handle(TRequest requestData, CancellationToken cancellationToken);
    }

    /// <summary>
    /// TBD
    /// </summary>
    public interface ITransactionalRequestHandler<TRequest, TResponse> : ITransactionalRequestHandler
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// TBD
        /// </summary>
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken);
    }
}