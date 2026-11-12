using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Hosting.Services
{
    /// <summary>
    /// Connects a generated Tycho application facade to a Microsoft host lifecycle.
    /// </summary>
    [ReferencedBySourceGenerator]
    public class AppHostedLifecycleService<TApp> : IHostedLifecycleService where TApp : IRunnable
    {
        private readonly TApp _app;

        /// <summary>
        /// Creates a lifecycle service for the supplied application.
        /// </summary>
        public AppHostedLifecycleService(TApp app)
        {
            _app = app;
        }

        /// <inheritdoc/>
        public Task StartingAsync(CancellationToken cancellationToken) => _app.StartAsync(cancellationToken);

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task StoppedAsync(CancellationToken cancellationToken) => _app.StopAsync(cancellationToken);
    }
}
