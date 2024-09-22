using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TychoV2.Structure;

namespace TychoV2.Requests.Setup
{
    internal class ContractBuilder
    {
        private readonly Internals _internals;

        public ContractBuilder(Internals internals)
        {
            _internals = internals;
        }

        // string ServiceKey

        // Handle
        // Forward
        // Expose

        private void RegisterRequestHandler<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            if (!TryRegisterRequestHandler<IHandle<TRequest>, THandler>())
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }

        private void RegisterRequestHandler<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            if (!TryRegisterRequestHandler<IHandle<TRequest, TResponse>, THandler>())
            {
                throw new ArgumentException($"Request handler for {typeof(TRequest).Name} already registered", nameof(THandler));
            }
        }

        private bool TryRegisterRequestHandler<THandlerInterface, THandler>()
            where THandlerInterface : class, IHandle
            where THandler : class, THandlerInterface
        {
            var services = _internals.GetServiceCollection();

            if (!services.Any(service => service.ServiceType == typeof(THandlerInterface)))
            {
                services.AddTransient<THandlerInterface, THandler>();
                return true;
            }

            return false;
        }
    }
}
