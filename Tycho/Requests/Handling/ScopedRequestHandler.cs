using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure.Data;

namespace Tycho.Requests.Handling
{
    internal class ScopedRequestHandler<TRequest, TRequestHandler> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TRequestHandler : IRequestHandler<TRequest>
    {
        private readonly Internals _internals;

        public ScopedRequestHandler(Internals internals)
        {
            _internals = internals;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            using var scope = _internals.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRequestHandler>();
            return handler.Handle(requestData, cancellationToken);
        }
    }

    internal class ScopedRequestHandler<TRequest, TResponse, TRequestHandler> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TRequestHandler : IRequestHandler<TRequest, TResponse>
    {
        private readonly Internals _internals;

        public ScopedRequestHandler(Internals internals)
        {
            _internals = internals;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            using var scope = _internals.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRequestHandler>();
            return handler.Handle(requestData, cancellationToken);
        }
    }
}