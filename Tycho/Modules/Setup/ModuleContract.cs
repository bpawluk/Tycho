using System;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Requests.Registrating;
using Tycho.Structure;

namespace Tycho.Modules.Setup
{
    internal class ModuleContract : IModuleContract
    {
        private readonly Registrator _registrator;

        private IRequestBroker? _contractFulfillingBroker;

        public IRequestBroker ContractFulfillingBroker => _contractFulfillingBroker ?? throw new InvalidOperationException("Contract fulfilling broker has not been defined yet.");

        public ModuleContract(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public void WithContractFulfillment(IRequestBroker contractFulfillingBroker)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
        }

        public IModuleRequestBinding<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest
        {
            return new ModuleRequestBinding<TRequest>(this, _registrator);
        }

        public IModuleRequestBinding<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new ModuleRequestBinding<TRequest, TResponse>(this, _registrator);
        }

        public IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest
        {
            if (!ContractFulfillingBroker.CanExecute<TRequest>())
            {
                throw new InvalidOperationException(
                    $"Parent module does not handle " +
                    $"the required {typeof(TRequest).Name} request");
            }
            return this;
        }

        public IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            if (!ContractFulfillingBroker.CanExecute<TRequest, TResponse>())
            {
                throw new InvalidOperationException(
                    $"Parent module does not handle " +
                    $"the required {typeof(TRequest).Name} request");
            }
            return this;
        }
    }
}
