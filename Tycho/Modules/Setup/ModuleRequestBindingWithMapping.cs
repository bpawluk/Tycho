using System;
using Tycho.Requests;
using Tycho.Requests.Registration;

namespace Tycho.Modules.Setup
{
    internal class ModuleRequestBindingWithMapping<TRequest, TTargetRequest> : IModuleRequestBindingWithMapping<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;

        public ModuleRequestBindingWithMapping(IModuleContract contract, Registrator registrator, Func<TRequest, TTargetRequest> mapRequest)
        {
            _contract = contract;
            _registrator = registrator;
            _mapRequest = mapRequest;
        }

        public IModuleContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpstreamRequest<TRequest, TTargetRequest, TModule>(_mapRequest);
            return _contract;
        }
    }

    internal class ModuleRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse> :
        IModuleRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public ModuleRequestBindingWithMapping(
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
            _registrator.ForwardMappedUpstreamRequest<
                TRequest, TResponse,
                TTargetRequest, TTargetResponse,
                TModule>(_mapRequest, _mapResponse);
            return _contract;
        }
    }
}
