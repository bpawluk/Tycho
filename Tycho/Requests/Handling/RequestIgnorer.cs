using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests.Handling
{
    internal class RequestIgnorer<TRequest> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
    {
        public Task HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class RequestIgnorer<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        public Task<TResponse> HandleAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(TResponse)!);
        }
    }
}
