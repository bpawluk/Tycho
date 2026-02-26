using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests.Broker;

namespace Tycho.Requests.Handling
{
    internal abstract class RequestRelay<TRequest> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IRequestBroker _targetBroker;

        public RequestRelay(IRequestBroker targetBroker)
        {
            _targetBroker = targetBroker;
        }

        public Task HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            return _targetBroker.ExecuteAsync(requestData, cancellationToken);
        }
    }

    internal abstract class RequestRelay<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IRequestBroker _targetBroker;

        public RequestRelay(IRequestBroker targetBroker)
        {
            _targetBroker = targetBroker;
        }

        public Task<TResponse> HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            return _targetBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
