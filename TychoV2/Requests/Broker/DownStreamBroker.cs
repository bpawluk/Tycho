﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Modules;
using TychoV2.Requests.Registrations;
using TychoV2.Structure;

namespace TychoV2.Requests.Broker
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
            var handlerRegistration = _internals.GetRequiredService<IDownStreamHandlerRegistration<TRequest, TModule>>();
            return handlerRegistration.Handler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            var handlerRegistration = _internals.GetRequiredService<IDownStreamHandlerRegistration<TRequest, TResponse, TModule>>();
            return handlerRegistration.Handler.Handle(requestData, cancellationToken);
        }
    }
}
