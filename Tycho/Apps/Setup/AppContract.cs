using System;
using System.Threading.Tasks;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps.Setup
{
    internal class AppContract : IAppContract
    {
        private readonly Registrator _registrator;

        public AppContract(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IAppRequestExpectation<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest
        {
            return new AppRequestExpectation<TRequest>(this, _registrator);
        }

        public IAppRequestExpectation<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new AppRequestExpectation<TRequest, TResponse>(this, _registrator);
        }

        public Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }

    internal class AppRequestExpectation<TRequest> : IAppRequestExpectation<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;

        public AppRequestExpectation(IAppContract contract, Registrator registrator)
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

        public IAppMappedRequestExpectation<TRequest, TTargetRequest> MapsTo<TTargetRequest>(
            Func<TRequest, TTargetRequest> mapRequest)
            where TTargetRequest : class, IRequest
        {
            mapRequest.ThrowIfNull();
            return new AppMappedRequestExpectation<TRequest, TTargetRequest>(
                _contract,
                _registrator,
                mapRequest);
        }
    }

    internal class AppRequestExpectation<TRequest, TResponse> : IAppRequestExpectation<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;

        public AppRequestExpectation(IAppContract contract, Registrator registrator)
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

        public IAppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> MapsTo<TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            mapRequest.ThrowIfNull();
            mapResponse.ThrowIfNull();
            return new AppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                _contract,
                _registrator,
                mapRequest,
                mapResponse);
        }
    }

    internal class AppMappedRequestExpectation<TRequest, TTargetRequest> :
        IAppMappedRequestExpectation<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;

        public AppMappedRequestExpectation(
            IAppContract contract,
            Registrator registrator,
            Func<TRequest, TTargetRequest> mapRequest)
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

    internal class AppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse> :
        IAppMappedRequestExpectation<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IAppContract _contract;
        private readonly Registrator _registrator;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public AppMappedRequestExpectation(
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
