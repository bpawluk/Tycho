using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;

namespace Tycho.Requests.Handling
{
    internal class MappedRequestForwarder<TRequest, TTargetRequest, TModule> 
        : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;
        private readonly Func<TRequest, TTargetRequest> _map;

        public MappedRequestForwarder(IModule<TModule> childModule, Func<TRequest, TTargetRequest> map)
        {
            _childModule = childModule;
            _map = map;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _map(requestData);
            return _childModule.Execute(targetRequestData, cancellationToken);
        }
    }

    internal class MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule> 
        : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public MappedRequestForwarder(
            IModule<TModule> childModule,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _childModule = childModule;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public async Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _mapRequest(requestData);
            var targetRequestResponse = await _childModule.Execute<TTargetRequest, TTargetResponse>(
                targetRequestData, cancellationToken);
            return _mapResponse(targetRequestResponse);
        }
    }
}