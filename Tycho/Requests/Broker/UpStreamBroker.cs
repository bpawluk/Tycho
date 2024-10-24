﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;

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
            if (requestData is null)
            {
                throw new ArgumentNullException(nameof(requestData), $"{nameof(requestData)} cannot be null");
            }

            var registration = _internals.GetRequiredService<IUpStreamHandlerRegistration<TRequest>>();
            return registration.Handler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            if (requestData is null)
            {
                throw new ArgumentNullException(nameof(requestData), $"{nameof(requestData)} cannot be null");
            }

            var registration = _internals.GetRequiredService<IUpStreamHandlerRegistration<TRequest, TResponse>>();
            return registration.Handler.Handle(requestData, cancellationToken);
        }
    }
}