using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Structure.Parent
{
    /// <summary>
    /// Base class for generated parent facades.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class ParentBase
    {
        private readonly IParentReference _parentReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentBase"/> class.
        /// </summary>
        /// <param name="parentReference">The parent reference used to execute requests.</param>
        [ReferencedBySourceGenerator]
        public ParentBase(IParentReference parentReference)
        {
            _parentReference = parentReference;
        }

        /// <summary>
        /// Executes a request that does not return a response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <param name="requestData">The request payload.</param>
        /// <param name="cancellationToken">A token that can cancel request execution.</param>
        /// <returns>A task that completes when the request has been handled.</returns>
        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        /// <summary>
        /// Executes a request that returns a response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="requestData">The request payload.</param>
        /// <param name="cancellationToken">A token that can cancel request execution.</param>
        /// <returns>A task that produces the request response.</returns>
        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
