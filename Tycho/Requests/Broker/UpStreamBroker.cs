using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Requests.Broker
{
    internal class UpStreamBroker : IRequestBroker
    {
        private readonly Internals _internals;

        public UpStreamBroker(Internals internals)
        {
            _internals = internals;
        }

        public bool CanExecute<TRequest>()
            where TRequest : class, IRequest
        {
            return _internals.HasService<IUpStreamRequestRegistration<TRequest>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IUpStreamRequestRegistration<TRequest, TResponse>>();
        }

        [EntryPoint]
        public async Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            await using var scope = _internals.CreateAsyncScope();
            var registration = scope.ServiceProvider.GetRequiredService<IUpStreamRequestRegistration<TRequest>>();
            await registration.Handler.HandleAsync(requestData, cancellationToken).ConfigureAwait(false);
        }

        [EntryPoint]
        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            await using var scope = _internals.CreateAsyncScope();
            var registration = scope.ServiceProvider.GetRequiredService<IUpStreamRequestRegistration<TRequest, TResponse>>();
            return await registration.Handler.HandleAsync(requestData, cancellationToken).ConfigureAwait(false);
        }
    }
}
