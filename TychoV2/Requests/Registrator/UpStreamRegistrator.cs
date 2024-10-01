using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TychoV2.Modules;
using TychoV2.Requests.Handling;
using TychoV2.Requests.Registrations;

namespace TychoV2.Requests.Registrator
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
            where THandler : class, IHandle<TRequest>
        {
            RegisterUpStreamRequestHandler<TRequest, THandler>();
        }

        public void HandleUpStreamRequest<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            RegisterUpStreamRequestHandler<TRequest, TResponse, THandler>();
        }

        private void RegisterUpStreamRequestHandler<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            if (TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest>,
                    UpStreamHandlerRegistration<TRequest, THandler>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }

        private void RegisterUpStreamRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            if (TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest, TResponse>,
                    UpStreamHandlerRegistration<TRequest, TResponse, THandler>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }
    }
}
