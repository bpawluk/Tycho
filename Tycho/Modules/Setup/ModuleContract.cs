using System;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Requests.Registrating;
using Tycho.Structure;
using Tycho.Utils;

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

        public IModuleRequestExpectation<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest
        {
            return new ModuleRequestExpectation<TRequest>(this, _registrator);
        }

        public IModuleRequestExpectation<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new ModuleRequestExpectation<TRequest, TResponse>(this, _registrator);
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

        public Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }

    internal class ModuleRequestExpectation<TRequest> : IModuleRequestExpectation<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;

        public ModuleRequestExpectation(IModuleContract contract, Registrator registrator)
        {
            _contract = contract;
            _registrator = registrator;
        }

        public IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return _contract;
        }

        public IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return _contract;
        }

        public IModuleMappedRequestExpectation<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest
        {
            mapRequest.ThrowIfNull();
            return new ModuleMappedRequestExpectation<TRequest, TTargetRequest>(
                _contract,
                _registrator,
                mapRequest);
        }
    }

    internal class ModuleRequestExpectation<TRequest, TResponse> : IModuleRequestExpectation<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;

        public ModuleRequestExpectation(IModuleContract contract, Registrator registrator)
        {
            _contract = contract;
            _registrator = registrator;
        }

        public IModuleContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return _contract;
        }

        public IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return _contract;
        }

        public IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            mapRequest.ThrowIfNull();
            mapResponse.ThrowIfNull();
            return new ModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                _contract,
                _registrator,
                mapRequest,
                mapResponse);
        }
    }

    internal class ModuleMappedRequestExpectation<TRequest, TTargetRequest> :
        IModuleMappedRequestExpectation<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;

        public ModuleMappedRequestExpectation(
            IModuleContract contract,
            Registrator registrator,
            Func<TRequest, TTargetRequest> mapRequest)
        {
            _contract = contract;
            _registrator = registrator;
            _mapRequest = mapRequest;
        }

        public IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpStreamRequest<TRequest, TTargetRequest, TModule>(_mapRequest);
            return _contract;
        }
    }

    internal class ModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> :
        IModuleMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public ModuleMappedRequestExpectation(
            IModuleContract contract,
            Registrator registrator,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _contract = contract;
            _registrator = registrator;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpStreamRequest<
                TRequest, TResponse,
                TTargetRequest, TTargetResponse,
                TModule>(_mapRequest, _mapResponse);
            return _contract;
        }
    }
}
