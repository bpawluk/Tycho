using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// Base interface for all Request Handlers.
    /// </summary>
    public interface IRequestHandler
    {
    }

    /// <summary>
    /// Request Handler for a Request of type <typeparamref name="TRequest"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
    public interface IRequestHandler<TRequest> : IRequestHandler
        where TRequest : class, IRequest
    {
        /// <summary>
        /// Handles a Request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="requestData">The data of the Request to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task HandleAsync(TRequest requestData, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Request Handler for a Request of type <typeparamref name="TRequest"/> with response
    /// <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the Request to handle.</typeparam>
    /// <typeparam name="TResponse">The type of the response to return.</typeparam>
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
        where TRequest : class, IRequest<TResponse>
    {
        /// <summary>
        /// Handles a Request of type <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="requestData">The data of the Request to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A response of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> HandleAsync(TRequest requestData, CancellationToken cancellationToken);
    }
}
