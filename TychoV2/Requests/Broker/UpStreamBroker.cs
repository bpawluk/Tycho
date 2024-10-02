﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Requests.Registrations;
using TychoV2.Structure;

namespace TychoV2.Requests.Broker
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
            return _internals.HasService<IUpStreamHandlerRegistration<TRequest>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IUpStreamHandlerRegistration<TRequest, TResponse>>();
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            var handlerRegistration = _internals.GetRequiredService<IUpStreamHandlerRegistration<TRequest>>();
            return handlerRegistration.Handler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            var handlerRegistration = _internals.GetRequiredService<IUpStreamHandlerRegistration<TRequest, TResponse>>();
            return handlerRegistration.Handler.Handle(requestData, cancellationToken);
        }
    }
}