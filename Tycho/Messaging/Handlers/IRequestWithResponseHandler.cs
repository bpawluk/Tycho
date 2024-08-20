using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    /// <summary>
    /// An interface that represents a request handler
    /// </summary>
    /// <typeparam name="Request">The type of the request being handled</typeparam>
    /// <typeparam name="Response">The type of the request response</typeparam>
    public interface IRequestHandler<in Request, Response> : IMessageHandler
        where Request : class, IRequest<Response>
    {
        /// <summary>
        /// A method to be executed when the specified request is received
        /// </summary>
        /// <param name="requestData">An object that represents the request being handled</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the request result</returns>
        Task<Response> Handle(Request requestData, CancellationToken cancellationToken = default);
    }
}
