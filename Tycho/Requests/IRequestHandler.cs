using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// Base interface for all request handlers
    /// </summary>
    public interface IRequestHandler
    {
    }

    /// <summary>
    /// Request handler for a request of type <typeparamref name="TRequest"/>
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to handle</typeparam>
    public interface IRequestHandler<TRequest> : IRequestHandler
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Handles a request of type <typeparamref name="TRequest"/>
        /// </summary>
        /// <param name="requestData">The data of the event to handle</param>
        Task Handle(TRequest requestData, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Request handler for a request of type <typeparamref name="TRequest"/> with response <typeparamref name="TResponse"/>
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to handle</typeparam>
    /// <typeparam name="TResponse">The type of the response to return</typeparam>
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Handles a request of type <typeparamref name="TRequest"/>
        /// </summary>
        /// <param name="requestData">The data of the request to handle</param>
        /// <returns>A response of type <typeparamref name="TResponse"/></returns>
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken);
    }
}