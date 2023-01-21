using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    /// <summary>
    /// An interface that represents a query message handler
    /// </summary>
    /// <typeparam name="Query">The type of the query being handled</typeparam>
    /// <typeparam name="Response">The type of the query response</typeparam>
    public interface IQueryHandler<in Query, Response> : IQueryHandler
        where Query : class, IQuery<Response>
    {
        /// <summary>
        /// A method to be executed when the specified query is received
        /// </summary>
        /// <param name="queryData">An object that represents the query being handled</param>
        /// <param name="cancellationToken">A token that notifies when the operation should be canceled</param>
        /// <returns>A task that represents the asynchronous operation which returns the query result</returns>
        Task<Response> Handle(Query queryData, CancellationToken cancellationToken = default);
    }

    public interface IQueryHandler { }
}
