using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Interceptors
{
    /// <summary>
    /// Lets you run additional logic before and after forwarding a query message
    /// </summary>
    /// <typeparam name="Query">The type of the query being intercepted</typeparam>
    /// <typeparam name="Response">The type of the query response</typeparam>
    public interface IQueryInterceptor<Query, Response> where Query : class, IQuery<Response>
    {
        /// <summary>
        /// A method to be executed before the specified query is forwarded
        /// </summary>
        /// <param name="queryData">An object that represents the query being intercepted</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ExecuteBefore(Query queryData, CancellationToken cancellationToken = default);

        /// <summary>
        /// A method to be executed after the specified query is forwarded
        /// </summary>
        /// <param name="queryData">An object that represents the query being intercepted</param>
        /// <param name="queryResult">Intercepted query result</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the query result</returns>
        Task<Response> ExecuteAfter(Query queryData, Response queryResult, CancellationToken cancellationToken = default);
    }
}
