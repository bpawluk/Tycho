using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TychoV2.Modules;
using TychoV2.Requests.Handling;
using TychoV2.Requests.Registrations;

namespace TychoV2.Requests.Registrator
{
    internal partial class Registrator
    {
        public void ExposeDownStreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, RequestExposer<TRequest>>();
        }

        public void ExposeDownStreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, TResponse, RequestExposer<TRequest, TResponse>>();
        }

        public void ForwardDownStreamRequest<TSourceModule, TRequest, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardDownStreamRequest<TSourceModule, TRequest, TResponse, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, TResponse, RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void HandleDownStreamRequest<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, THandler>();
        }

        public void HandleDownStreamRequest<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, TResponse, THandler>();
        }

        public void IgnoreDownStreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, RequestIgnorer<TRequest>>();
        }

        public void IgnoreDownStreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            RegisterDownStreamRequestHandler<TSourceModule, TRequest, TResponse, RequestIgnorer<TRequest, TResponse>>();
        }

        private void RegisterDownStreamRequestHandler<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            if (TryAddRegistration<
                    IDownStreamHandlerRegistration<TRequest, TSourceModule>,
                    DownStreamHandlerRegistration<TRequest, THandler, TSourceModule>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }

        private void RegisterDownStreamRequestHandler<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            if (TryAddRegistration<
                    IDownStreamHandlerRegistration<TRequest, TResponse, TSourceModule>,
                    DownStreamHandlerRegistration<TRequest, TResponse, THandler, TSourceModule>>())
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
