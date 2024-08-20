using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Interceptors
{
    /// <summary>
    /// Lets you run additional logic before and after forwarding a request
    /// </summary>
    /// <typeparam name="Request">The type of the request being intercepted</typeparam>
    /// <typeparam name="Response">The type of the request response</typeparam>
    public interface IRequestInterceptor<Request, Response> where Request : class, IRequest<Response>
    {
        /// <summary>
        /// A method to be executed before the specified request is forwarded
        /// </summary>
        /// <param name="requestData">An object that represents the request being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteBefore(Request requestData, CancellationToken cancellationToken = default);

        /// <summary>
        /// A method to be executed after the specified request is forwarded
        /// </summary>
        /// <param name="requestData">An object that represents the request being intercepted</param>
        /// <param name="requestResult">Intercepted request result</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the request result</returns>
        Task<Response> ExecuteAfter(Request requestData, Response requestResult, CancellationToken cancellationToken = default);
    }
}
