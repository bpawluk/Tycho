using System;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Utils;

namespace Tycho.Modules.Setup
{
    internal class ModuleRequestBinding<TRequest> : IModuleRequestBinding<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;

        public ModuleRequestBinding(IModuleContract contract, Registrator registrator)
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

        public IModuleRequestBindingWithMapping<TRequest, TTargetRequest> MapsTo<TTargetRequest>(Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest
        {
            mapRequest.ThrowIfNull();
            return new ModuleRequestBindingWithMapping<TRequest, TTargetRequest>(_contract, _registrator, mapRequest);
        }
    }

    internal class ModuleRequestBinding<TRequest, TResponse> : IModuleRequestBinding<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IModuleContract _contract;
        private readonly Registrator _registrator;

        public ModuleRequestBinding(IModuleContract contract, Registrator registrator)
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

        public IModuleRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            mapRequest.ThrowIfNull();
            mapResponse.ThrowIfNull();
            return new ModuleRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                _contract,
                _registrator,
                mapRequest,
                mapResponse);
        }
    }
}
