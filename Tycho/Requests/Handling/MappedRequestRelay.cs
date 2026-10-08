using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests.Broker;

namespace Tycho.Requests.Handling
{
    internal abstract class MappedRequestRelay<TRequest, TTargetRequest>
        : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        private readonly IRequestBroker _targetBroker;
        private readonly Func<TRequest, TTargetRequest> _map;

        public MappedRequestRelay(IRequestBroker targetBroker, Func<TRequest, TTargetRequest> map)
        {
            _targetBroker = targetBroker;
            _map = map;
        }

        public Task HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            TTargetRequest targetRequestData = _map(requestData);
            return _targetBroker.ExecuteAsync(targetRequestData, cancellationToken);
        }
    }

    internal abstract class MappedRequestRelay<TRequest, TResponse, TTargetRequest, TTargetResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        private readonly IRequestBroker _targetBroker;
        private readonly Func<TRequest, TTargetRequest> _mapRequest;
        private readonly Func<TTargetResponse, TResponse> _mapResponse;

        public MappedRequestRelay(
            IRequestBroker targetBroker,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
        {
            _targetBroker = targetBroker;
            _mapRequest = mapRequest;
            _mapResponse = mapResponse;
        }

        public async Task<TResponse> HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            TTargetRequest targetRequestData = _mapRequest(requestData);
            TTargetResponse targetRequestResponse = await _targetBroker
                .ExecuteAsync<TTargetRequest, TTargetResponse>(
                    targetRequestData,
                    cancellationToken)
                .ConfigureAwait(false);
            return _mapResponse(targetRequestResponse);
        }
    }
}
