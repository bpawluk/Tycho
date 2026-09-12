using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests.Pipeline;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.Utils;

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
            return _internals.HasService<IUpStreamRequestRegistration<TRequest>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IUpStreamRequestRegistration<TRequest, TResponse>>();
        }

        [EntryPoint]
        public async Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();

            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IUpStreamRequestRegistration<TRequest> registration = scope.ServiceProvider.GetRequiredService<IUpStreamRequestRegistration<TRequest>>();
            RequestPipeline<TRequest, NoResponse> pipeline = RequestPipelineBuilder.Build(scope.ServiceProvider, registration.Handler);
            await pipeline.ExecuteAsync(requestData, cancellationToken).ConfigureAwait(false);
        }

        [EntryPoint]
        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();

            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            IUpStreamRequestRegistration<TRequest, TResponse> registration = scope.ServiceProvider.GetRequiredService<IUpStreamRequestRegistration<TRequest, TResponse>>();
            RequestPipeline<TRequest, TResponse> pipeline = RequestPipelineBuilder.Build(scope.ServiceProvider, registration.Handler);
            return await pipeline.ExecuteAsync(requestData, cancellationToken).ConfigureAwait(false);
        }
    }
}
