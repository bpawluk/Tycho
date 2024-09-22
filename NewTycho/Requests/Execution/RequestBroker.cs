using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Requests.Execution
{
    internal class RequestBroker : IExecute
    {
        private readonly IServiceProvider _services;

        public RequestBroker(IServiceProvider services)
        {
            _services = services;
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            if (requestData is null)
            {
                throw new ArgumentException($"{nameof(requestData)} cannot be null", nameof(requestData));
            }

            var requestHandler = _services.GetService<IHandle<TRequest>>();

            if (requestHandler is null)
            {
                throw new InvalidOperationException($"No handler found for request of type {typeof(TRequest).Name}");
            }

            return requestHandler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            if (requestData is null)
            {
                throw new ArgumentException($"{nameof(requestData)} cannot be null", nameof(requestData));
            }

            var requestHandler = _services.GetService<IHandle<TRequest, TResponse>>();

            if (requestHandler is null)
            {
                throw new InvalidOperationException($"No handler found for request of type {typeof(TRequest).Name}");
            }

            return requestHandler.Handle(requestData, cancellationToken);
        }
    }
}
