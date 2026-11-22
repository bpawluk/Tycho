using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Utils;

namespace Tycho.Hosting.Services
{
    internal sealed class HostLifecycleCallbacksService : IHostedLifecycleService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHostLifecycleCallbacks _callbacks;

        public HostLifecycleCallbacksService(IServiceScopeFactory scopeFactory, IHostLifecycleCallbacks callbacks)
        {
            _scopeFactory = scopeFactory;
            _callbacks = callbacks;
        }

        [EntryPoint]
        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            await _callbacks.Startup(scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        [EntryPoint]
        public async Task StoppedAsync(CancellationToken cancellationToken)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            await _callbacks.Cleanup(scope.ServiceProvider, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
