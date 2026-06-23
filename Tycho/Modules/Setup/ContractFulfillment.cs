using System;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Modules.Setup
{
    internal class ContractFulfillment<TSourceModule> : IContractFulfillment
        where TSourceModule : TychoModule
    {
        private readonly Registrator _registrator;

        public ContractFulfillment(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IContractRequestFulfillment<TRequest> Fulfills<TRequest>()
            where TRequest : class, IRequest
        {
            return new RequestFulfillment<TRequest>(this, _registrator);
        }

        public IContractRequestFulfillment<TRequest, TResponse> Fulfills<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new RequestFulfillment<TRequest, TResponse>(this, _registrator);
        }

        private class RequestFulfillment<TRequest> : IContractRequestFulfillment<TRequest>
            where TRequest : class, IRequest
        {
            private readonly IContractFulfillment _root;
            private readonly Registrator _registrator;

            public RequestFulfillment(IContractFulfillment root, Registrator registrator)
            {
                _root = root;
                _registrator = registrator;
            }

            public IContractFulfillment Exposes()
            {
                _registrator.ExposeDownStreamRequest<TSourceModule, TRequest>();
                return _root;
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

            public IMappedContractRequestFulfillment<TTargetRequest> MapsTo<TTargetRequest>(
                Func<TRequest, TTargetRequest> mapRequest)
                where TTargetRequest : class, IRequest
            {
                mapRequest.ThrowIfNull();
                return new MappedRequestFulfillment<TRequest, TTargetRequest>(_root, _registrator, mapRequest);
            }
        }

        private class RequestFulfillment<TRequest, TResponse> : IContractRequestFulfillment<TRequest, TResponse>
            where TRequest : class, IRequest<TResponse>
        {
            private readonly IContractFulfillment _root;
            private readonly Registrator _registrator;

            public RequestFulfillment(IContractFulfillment root, Registrator registrator)
            {
                _root = root;
                _registrator = registrator;
            }

            public IContractFulfillment Exposes()
            {
                _registrator.ExposeDownStreamRequest<TSourceModule, TRequest, TResponse>();
                return _root;
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

            public IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
                Func<TRequest, TTargetRequest> mapRequest,
                Func<TTargetResponse, TResponse> mapResponse)
                where TTargetRequest : class, IRequest<TTargetResponse>
            {
                mapRequest.ThrowIfNull();
                mapResponse.ThrowIfNull();
                return new MappedRequestFulfillment<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                    _root,
                    _registrator,
                    mapRequest,
                    mapResponse);
            }
        }

        private class MappedRequestFulfillment<TRequest, TTargetRequest>
            : IMappedContractRequestFulfillment<TTargetRequest>
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
        {
            private readonly IContractFulfillment _root;
            private readonly Registrator _registrator;
            private readonly Func<TRequest, TTargetRequest> _mapRequest;

            public MappedRequestFulfillment(
                IContractFulfillment root,
                Registrator registrator,
                Func<TRequest, TTargetRequest> mapRequest)
            {
                _root = root;
                _registrator = registrator;
                _mapRequest = mapRequest;
            }

            public IContractFulfillment Exposes()
            {
                _registrator.ExposeMappedDownStreamRequest<TSourceModule, TRequest, TTargetRequest>(_mapRequest);
                return _root;
            }

            public IContractFulfillment ForwardsTo<TModule>()
                where TModule : TychoModule
            {
                _registrator.ForwardMappedDownStreamRequest<TSourceModule, TRequest, TTargetRequest, TModule>(
                    _mapRequest);
                return _root;
            }
        }

        private class MappedRequestFulfillment<TRequest, TResponse, TTargetRequest, TTargetResponse>
            : IMappedContractRequestFulfillment<TTargetRequest, TTargetResponse>
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            private readonly IContractFulfillment _root;
            private readonly Registrator _registrator;
            private readonly Func<TRequest, TTargetRequest> _mapRequest;
            private readonly Func<TTargetResponse, TResponse> _mapResponse;

            public MappedRequestFulfillment(
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

            public IContractFulfillment Exposes()
            {
                _registrator.ExposeMappedDownStreamRequest<
                    TSourceModule,
                    TRequest, TResponse,
                    TTargetRequest, TTargetResponse>(_mapRequest, _mapResponse);
                return _root;
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
}
