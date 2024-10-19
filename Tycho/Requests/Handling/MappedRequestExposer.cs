using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;

namespace Tycho.Requests.Handling
{
    internal class MappedRequestExposer<TRequest, TTargetRequest> 
        : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IParent _parent;
        private readonly Func<TRequest, TTargetRequest> _map;

        public MappedRequestExposer(IParent parent, Func<TRequest, TTargetRequest> map)
        {
            _parent = parent;
            _map = map;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _map(requestData);
            return _parent.Execute(targetRequestData, cancellationToken);
        }
    }

    internal class MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse> 
        : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IParent _parent;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public MappedRequestExposer(
            IParent parent, 
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _parent = parent;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public async Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _mapRequest(requestData);
            var targetRequestResponse = await _parent.Execute<TTargetRequest, TTargetResponse>(
                targetRequestData, cancellationToken);
            return _mapResponse(targetRequestResponse);
        }
    }
}