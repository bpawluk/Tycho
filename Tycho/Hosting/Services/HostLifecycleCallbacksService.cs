using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        public async Task StartingAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await _callbacks.Startup(scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task StoppedAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await _callbacks.Cleanup(scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
        }
    }
}
