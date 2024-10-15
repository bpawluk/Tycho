using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Modules;
using Tycho.Requests.Handling;
using Tycho.Requests.Registrating.Registrations;

namespace Tycho.Requests.Registrating
{
    internal partial class Registrator
    {
        public void ForwardUpStreamRequest<TRequest, TTargetModule>()
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            RegisterUpStreamRequestHandler<TRequest, RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardUpStreamRequest<TRequest, TResponse, TTargetModule>()
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            RegisterUpStreamRequestHandler<TRequest, TResponse, RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void HandleUpStreamRequest<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            RegisterUpStreamRequestHandler<TRequest, THandler>();
        }

        public void HandleUpStreamRequest<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            RegisterUpStreamRequestHandler<TRequest, TResponse, THandler>();
        }

        private void RegisterUpStreamRequestHandler<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest>,
                    UpStreamHandlerRegistration<TRequest, THandler>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void RegisterUpStreamRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest, TResponse>,
                    UpStreamHandlerRegistration<TRequest, TResponse, THandler>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }
    }
}