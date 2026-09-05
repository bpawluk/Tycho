using System;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Utils;

namespace Tycho.Apps.Setup
{
    internal class RequiredRequestBinding<TSourceModule, TRequest> : IRequiredRequestBinding<TRequest>
        where TSourceModule : TychoModule
        where TRequest : class, IRequest
    {
        private readonly IContractFulfillment _root;
        private readonly Registrator _registrator;

        public RequiredRequestBinding(IContractFulfillment root, Registrator registrator)
        {
            _root = root;
            _registrator = registrator;
        }

        public IContractFulfillment Ignores()
        {
            _registrator.IgnoreDownStreamRequest<TSourceModule, TRequest>();
            return _root;
        }

        public IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, THandler>();
            return _root;
        }

        public IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardDownStreamRequest<TSourceModule, TRequest, TModule>();
            return _root;
        }

        public IRequiredRequestBindingWithMapping<TTargetRequest> MapsTo<TTargetRequest>(Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest
        {
            mapRequest.ThrowIfNull();
            return new RequiredRequestBindingWithMapping<TSourceModule, TRequest, TTargetRequest>(_root, _registrator, mapRequest);
        }
    }

    internal class RequiredRequestBinding<TSourceModule, TRequest, TResponse> : IRequiredRequestBinding<TRequest, TResponse>
        where TSourceModule : TychoModule
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IContractFulfillment _root;
        private readonly Registrator _registrator;

        public RequiredRequestBinding(IContractFulfillment root, Registrator registrator)
        {
            _root = root;
            _registrator = registrator;
        }

        public IContractFulfillment Ignores()
        {
            _registrator.IgnoreDownStreamRequest<TSourceModule, TRequest, TResponse>();
            return _root;
        }

        public IContractFulfillment HandlesWith<THandler>()
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, TResponse, THandler>();
            return _root;
        }

        public IContractFulfillment ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardDownStreamRequest<TSourceModule, TRequest, TResponse, TModule>();
            return _root;
        }

        public IRequiredRequestBindingWithMapping<TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            mapRequest.ThrowIfNull();
            mapResponse.ThrowIfNull();
            return new RequiredRequestBindingWithMapping<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse>(
                _root,
                _registrator,
                mapRequest,
                mapResponse);
        }
    }
}
