using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TychoV2.Modules;
using TychoV2.Requests.Registrations;

namespace TychoV2.Requests.Registrator
{
    internal partial class Registrator
    {
        // Handle (up/down)
        // Forward (up/down)
        // Expose (down)

        private void RegisterDownStreamRequestHandler<TRequest, THandler, TModule>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
            where TModule : TychoModule
        {
            if (TryAddRegistration<IDownStreamHandlerRegistration<TRequest, TModule>, DownStreamHandlerRegistration<TRequest, THandler, TModule>>())
            {
                Services.TryAddTransient<THandler>();
            }
            else
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }

        private void RegisterDownStreamRequestHandler<TRequest, TResponse, THandler, TModule>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
            where TModule : TychoModule
        {
            if (TryAddRegistration<IDownStreamHandlerRegistration<TRequest, TResponse, TModule>, DownStreamHandlerRegistration<TRequest, TResponse, THandler, TModule>>())
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
