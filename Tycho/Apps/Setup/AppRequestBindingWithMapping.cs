using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;

namespace Tycho.Apps.Setup
{
    internal class AppRequestBindingWithMapping<TRequest, TTargetRequest> : IAppRequestBindingWithMapping<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;

        public AppRequestBindingWithMapping(IAppContract contract, Registrator registrator, Func<TRequest, TTargetRequest> mapRequest)
        {
            _contract = contract;
            _registrator = registrator;
            _mapRequest = mapRequest;
        }

        public IAppContract ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedUpStreamRequest<TRequest, TTargetRequest, TModule>(_mapRequest);
            return _contract;
        }
    }

    internal class AppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse> : IAppRequestBindingWithMapping<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public AppRequestBindingWithMapping(
            IAppContract contract,
            Registrator registrator,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _contract = contract;
            _registrator = registrator;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public IAppContract ForwardsTo<TModule>()
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
