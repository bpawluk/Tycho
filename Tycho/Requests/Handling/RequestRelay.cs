using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests.Handling
{
    internal abstract class RequestRelay<TRequest> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IRequestExecutor _targetExecutor;

        public RequestRelay(IRequestExecutor targetExecutor)
        {
            _targetExecutor = targetExecutor;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _targetExecutor.Execute(requestData, cancellationToken);
        }
    }

    internal abstract class RequestRelay<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IRequestExecutor _targetExecutor;

        public RequestRelay(IRequestExecutor targetExecutor)
        {
            _targetExecutor = targetExecutor;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _targetExecutor.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
