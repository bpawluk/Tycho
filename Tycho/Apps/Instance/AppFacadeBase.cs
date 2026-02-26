using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    [ReferencedBySourceGenerator]
    public abstract class AppFacadeBase : IAsyncDisposable
    {
        private readonly IApp _app;

        [ReferencedBySourceGenerator]
        public AppFacadeBase(IApp app)
        {
            _app = app;
        }

        [ReferencedBySourceGenerator]
        protected Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull(nameof(requestData));
            return _app.RequestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        [ReferencedBySourceGenerator]
        protected Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull(nameof(requestData));
            return _app.RequestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }

        public ValueTask DisposeAsync() => _app.DisposeAsync();
    }
}
