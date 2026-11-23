using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// Intercepts a request before it reaches its registered handler.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    public interface IRequestInterceptor<TRequest, TResponse>
        where TRequest : class
    {
        /// <summary>
        /// Intercepts a request before it reaches its registered handler.
        /// </summary>
        /// <param name="next">The next stage in the request handling pipeline.</param>
        /// <param name="requestData">The current request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The response to return.</returns>
        Task<TResponse> InterceptAsync(
            RequestHandlerDelegate<TRequest, TResponse> next,
            TRequest requestData,
            CancellationToken cancellationToken);
    }
}
