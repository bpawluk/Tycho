using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;

namespace Tycho.Apps.Setup
{
    internal class RequiredRequestBindingWithMapping<TSourceModule, TRequest, TTargetRequest> : IRequiredRequestBindingWithMapping<TTargetRequest>
        where TSourceModule : TychoModule
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IContractFulfillment _root;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;

        public RequiredRequestBindingWithMapping(IContractFulfillment root, Registrator registrator, Func<TRequest, TTargetRequest> mapRequest)
        {
            _root = root;
            _registrator = registrator;
            _mapRequest = mapRequest;
        }

        public IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedDownStreamRequest<TSourceModule, TRequest, TTargetRequest, TModule>(_mapRequest);
            return _root;
        }
    }

    internal class RequiredRequestBindingWithMapping<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse> :
        IRequiredRequestBindingWithMapping<TTargetRequest, TTargetResponse>
        where TSourceModule : TychoModule
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IContractFulfillment _root;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public RequiredRequestBindingWithMapping(
            IContractFulfillment root,
            Registrator registrator,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _root = root;
            _registrator = registrator;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardMappedDownStreamRequest<
                TSourceModule,
                TRequest, TResponse,
                TTargetRequest, TTargetResponse,
                TModule>(_mapRequest, _mapResponse);
            return _root;
        }
    }
}
