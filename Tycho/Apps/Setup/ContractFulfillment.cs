using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Structure;

namespace Tycho.Apps.Setup
{
    internal class ContractFulfillment<TSourceModule> : IContractFulfillment
        where TSourceModule : TychoModule
    {
        private readonly Registrator _registrator;

        public ContractFulfillment(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IRequiredRequestBinding<TRequest> Fulfills<TRequest>()
            where TRequest : class, IRequest
        {
            return new RequiredRequestBinding<TSourceModule, TRequest>(this, _registrator);
        }

        public IRequiredRequestBinding<TRequest, TResponse> Fulfills<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new RequiredRequestBinding<TSourceModule, TRequest, TResponse>(this, _registrator);
        }
    }
}
