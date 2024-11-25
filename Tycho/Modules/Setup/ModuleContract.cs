using System;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Requests.Registrating;
using Tycho.Structure.Data;

namespace Tycho.Modules.Setup
{
    internal class ModuleContract : IModuleContract
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        private IRequestBroker? _contractFulfillingBroker;

        public IRequestBroker ContractFulfillingBroker => _contractFulfillingBroker ??
            throw new InvalidOperationException("Contract fulfilling broker has not been defined yet.");

        public ModuleContract(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(_internals);
        }

        public void WithContractFulfillment(IRequestBroker contractFulfillingBroker)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
        }

        public IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return this;
        }

        public IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return this;
        }

        public IModuleContract ForwardsAs<TRequest, TTargetRequest, TModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpStreamRequest<TRequest, TTargetRequest, TModule>(map);
            return this;
        }

        public IModuleContract ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpStreamRequest<
                TRequest, TResponse,
                TTargetRequest, TTargetResponse,
                TModule>(mapRequest, mapResponse);
            return this;
        }

        public IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return this;
        }

        public IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return this;
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

        public Task Build()
        {
            return Task.CompletedTask;
        }
    }
}