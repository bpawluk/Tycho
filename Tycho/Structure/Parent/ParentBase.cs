using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Structure.Parent
{
    [ReferencedBySourceGenerator]
    public abstract class ParentBase
    {
        private readonly IParentReference _parentReference;

        [ReferencedBySourceGenerator]
        public ParentBase(IParentReference parentReference)
        {
            _parentReference = parentReference;
        }

        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _parentReference.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
