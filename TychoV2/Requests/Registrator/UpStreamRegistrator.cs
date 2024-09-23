using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TychoV2.Requests.Registrations;

namespace TychoV2.Requests.Registrator
{
    internal partial class Registrator
    {
        // Handle (up/down)
        // Forward (up/down)

        private void RegisterUpStreamRequestHandler<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            if (TryAddRegistration<IUpStreamHandlerRegistration<TRequest>, UpStreamHandlerRegistration<TRequest, THandler>>())
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
            if (TryAddRegistration<IUpStreamHandlerRegistration<TRequest, TResponse>, UpStreamHandlerRegistration<TRequest, TResponse, THandler>>())
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
