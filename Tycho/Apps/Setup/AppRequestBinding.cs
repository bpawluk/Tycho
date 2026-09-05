using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Utils;

namespace Tycho.Apps.Setup
{
    internal class AppRequestBinding<TRequest> : IAppRequestBinding<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;

        public AppRequestBinding(IAppContract contract, Registrator registrator)
        {
            _contract = contract;
            _registrator = registrator;
        }

        public IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return _contract;
        }

        public IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return _contract;
        }

        public IAppRequestBindingWithMapping<TRequest, TTargetRequest> MapsTo<TTargetRequest>(Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest
        {
            mapRequest.ThrowIfNull();
            return new AppRequestBindingWithMapping<TRequest, TTargetRequest>(_contract, _registrator, mapRequest);
        }
    }

    internal class AppRequestBinding<TRequest, TResponse> : IAppRequestBinding<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;

        public AppRequestBinding(IAppContract contract, Registrator registrator)
        {
            _contract = contract;
            _registrator = registrator;
        }

        public IAppContract HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return _contract;
        }

        public IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return _contract;
        }

        public IAppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            mapRequest.ThrowIfNull();
            mapResponse.ThrowIfNull();
            return new AppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                _contract,
                _registrator,
                mapRequest,
                mapResponse);
        }
    }
}
