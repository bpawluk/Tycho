using Tycho.Requests;

namespace Tycho.Transactions
{
    /// <summary>
    /// Base interface for request handlers that support transactional behavior.
    /// </summary>
    public interface ITransactionalRequestHandler : IRequestHandler
    {
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
