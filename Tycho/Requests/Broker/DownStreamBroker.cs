using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;

namespace Tycho.Requests.Broker
{
    internal class DownStreamBroker<TModule> : IRequestBroker
        where TModule : TychoModule
    {
        private readonly Internals _internals;

        public DownStreamBroker(Internals internals)
        {
            _internals = internals;
        }

        public bool CanExecute<TRequest>()
            where TRequest : class, IRequest
        {
            return _internals.HasService<IDownStreamRequestRegistration<TRequest, TModule>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IDownStreamRequestRegistration<TRequest, TResponse, TModule>>();
        }

        public Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            var registration = _internals.GetRequiredService<IDownStreamRequestRegistration<TRequest, TModule>>();
            return registration.Handler.HandleAsync(requestData, cancellationToken);
        }

        public Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            var registration = _internals.GetRequiredService<IDownStreamRequestRegistration<TRequest, TResponse, TModule>>();
            return registration.Handler.HandleAsync(requestData, cancellationToken);
        }
    }
}
