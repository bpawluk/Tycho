using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests.Handling
{
    internal abstract class MappedRequestRelay<TRequest, TTargetRequest>
        : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IRequestExecutor _targetExecutor;
        private readonly Func<TRequest, TTargetRequest> _map;

        public MappedRequestRelay(IRequestExecutor targetExecutor, Func<TRequest, TTargetRequest> map)
        {
            _targetExecutor = targetExecutor;
            _map = map;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _map(requestData);
            return _targetExecutor.Execute(targetRequestData, cancellationToken);
        }
    }

    internal abstract class MappedRequestRelay<TRequest, TResponse, TTargetRequest, TTargetResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IRequestExecutor _targetExecutor;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public MappedRequestRelay(
            IRequestExecutor targetExecutor,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _targetExecutor = targetExecutor;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public async Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            var targetRequestData = _mapRequest(requestData);
            var targetRequestResponse = await _targetExecutor
                .Execute<TTargetRequest, TTargetResponse>(
                    targetRequestData, 
                    cancellationToken)
                .ConfigureAwait(false);
            return _mapResponse(targetRequestResponse);
        }
    }
}