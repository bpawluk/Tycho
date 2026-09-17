using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Hosting
{
    internal sealed class HostLifecycleCallbacks : IHostLifecycleCallbacks
    {
        public Func<IServiceProvider, CancellationToken, Task> Startup { get; private set; } = Default;

        public Func<IServiceProvider, CancellationToken, Task> Cleanup { get; private set; } = Default;

        public void WithStartup(Func<IServiceProvider, CancellationToken, Task> startup)
        {
            Startup = startup ?? throw new ArgumentNullException(nameof(startup));
        }

        public void WithCleanup(Func<IServiceProvider, CancellationToken, Task> cleanup)
        {
            Cleanup = cleanup ?? throw new ArgumentNullException(nameof(cleanup));
        }

        private static Task Default(IServiceProvider services, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
