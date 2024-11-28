using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure.Internal;

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
            return _internals.HasService<IDownStreamHandlerRegistration<TRequest, TModule>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IDownStreamHandlerRegistration<TRequest, TResponse, TModule>>();
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            if (requestData is null)
            {
                throw new ArgumentNullException(nameof(requestData), $"{nameof(requestData)} cannot be null");
            }
            var registration = _internals.GetRequiredService<IDownStreamHandlerRegistration<TRequest, TModule>>();
            return registration.Handler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            if (requestData is null)
            {
                throw new ArgumentNullException(nameof(requestData), $"{nameof(requestData)} cannot be null");
            }
            var registration = _internals.GetRequiredService<IDownStreamHandlerRegistration<TRequest, TResponse, TModule>>();
            return registration.Handler.Handle(requestData, cancellationToken);
        }
    }
}