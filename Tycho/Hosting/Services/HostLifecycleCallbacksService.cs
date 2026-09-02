using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tycho.Hosting.Services
{
    internal sealed class HostLifecycleCallbacksService : IHostedLifecycleService
    {
        private readonly IServiceProvider _services;
        private readonly IHostLifecycleCallbacks _callbacks;

        public HostLifecycleCallbacksService(IServiceProvider services, IHostLifecycleCallbacks callbacks)
        {
            _services = services;
            _callbacks = callbacks;
        }

        public Task StartingAsync(CancellationToken cancellationToken) => _callbacks.Startup(_services, cancellationToken);

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => _callbacks.Cleanup(_services, cancellationToken);
    }
}
