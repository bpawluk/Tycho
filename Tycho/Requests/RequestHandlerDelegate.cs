using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// Represents the next stage in a request handling pipeline.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    /// <param name="requestData">The request passed to the next stage.</param>
    /// <param name="cancellationToken">The cancellation token passed to the next stage.</param>
    public delegate Task<TResponse> RequestHandlerDelegate<TRequest, TResponse>(
        TRequest requestData,
        CancellationToken cancellationToken)
        where TRequest : class;
}
