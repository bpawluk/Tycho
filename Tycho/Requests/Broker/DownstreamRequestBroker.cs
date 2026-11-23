using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests.Pipeline;
using Tycho.Requests.Registration.Registrations;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Requests.Broker
{
    internal class DownstreamRequestBroker<TModule> : IRequestBroker
        where TModule : TychoModule
    {
        private readonly Internals _internals;

        public DownstreamRequestBroker(Internals internals)
        {
            _internals = internals;
        }

        public bool CanExecute<TRequest>()
            where TRequest : class, IRequest
        {
            return _internals.HasService<IDownstreamRequestRegistration<TRequest, TModule>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IDownstreamRequestRegistration<TRequest, TResponse, TModule>>();
        }

        [EntryPoint]
        public async Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();
            cancellationToken.ThrowIfCancellationRequested();

            AsyncServiceScope scope = _internals.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            IDownstreamRequestRegistration<TRequest, TModule> registration = scope.ServiceProvider.GetRequiredService<IDownstreamRequestRegistration<TRequest, TModule>>();
            RequestPipeline<TRequest, NoResponse> pipeline = RequestPipelineBuilder.Build(scope.ServiceProvider, registration.Handler);
            await pipeline.ExecuteAsync(requestData, cancellationToken).ConfigureAwait(false);
        }

        [EntryPoint]
        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();
            cancellationToken.ThrowIfCancellationRequested();

            AsyncServiceScope scope = _internals.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            IDownstreamRequestRegistration<TRequest, TResponse, TModule> registration = scope.ServiceProvider.GetRequiredService<IDownstreamRequestRegistration<TRequest, TResponse, TModule>>();
            RequestPipeline<TRequest, TResponse> pipeline = RequestPipelineBuilder.Build(scope.ServiceProvider, registration.Handler);
            return await pipeline.ExecuteAsync(requestData, cancellationToken).ConfigureAwait(false);
        }
    }
}
