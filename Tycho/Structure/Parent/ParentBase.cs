using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Structure.Parent
{
    /// <summary>
    /// Base class for generated Parent facades.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class ParentBase
    {
        private readonly IParentReference _parentReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentBase"/> class.
        /// </summary>
        /// <param name="parentReference">The Parent reference used to execute Requests.</param>
        [ReferencedBySourceGenerator]
        public ParentBase(IParentReference parentReference)
        {
            _parentReference = parentReference;
        }

        /// <summary>
        /// Executes a Request that does not return a Response.
        /// </summary>
        /// <typeparam name="TRequest">The Request type.</typeparam>
        /// <param name="requestData">The Request payload.</param>
        /// <param name="cancellationToken">A token that can cancel Request execution.</param>
        /// <returns>A task that completes when the Request has been handled.</returns>
        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        /// <summary>
        /// Executes a Request that returns a Response.
        /// </summary>
        /// <typeparam name="TRequest">The Request type.</typeparam>
        /// <typeparam name="TResponse">The Response type.</typeparam>
        /// <param name="requestData">The Request payload.</param>
        /// <param name="cancellationToken">A token that can cancel Request execution.</param>
        /// <returns>A task that produces the Request Response.</returns>
        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
