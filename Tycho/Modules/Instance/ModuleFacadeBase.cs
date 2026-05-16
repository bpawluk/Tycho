using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    [ReferencedBySourceGenerator]
    public abstract class ModuleFacadeBase
    {
        private readonly IModule _module;

        [ReferencedBySourceGenerator]
        public ModuleFacadeBase(IModule _module)
        {
            this._module = _module;
        }

        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            return _module.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            return _module.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
