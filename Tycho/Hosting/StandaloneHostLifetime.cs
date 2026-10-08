using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tycho.Hosting
{
    internal sealed class StandaloneHostLifetime : IHostLifetime
    {
        public Task WaitForStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
