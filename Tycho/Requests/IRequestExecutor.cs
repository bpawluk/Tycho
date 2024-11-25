using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// An interface for executing requests
    /// </summary>
    public interface IRequestExecutor
    {
        /// <summary>
        /// Executes a request of type <typeparamref name="TRequest"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to execute</typeparam>
        /// <param name="requestData">The data of the request to execute</param>
        Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest;

        /// <summary>
        /// Executes a request of type <typeparamref name="TRequest"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to execute</typeparam>
        /// <typeparam name="TResponse">The type of the response to execute</typeparam>
        /// <param name="requestData">The data of the request to execute</param>
        /// <returns>A response of type <typeparamref name="TResponse"/></returns>
        Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest<TResponse>;
    }
}