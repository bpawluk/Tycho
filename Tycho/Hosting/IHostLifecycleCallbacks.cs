using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Hosting
{
    internal interface IHostLifecycleCallbacks
    {
        Func<IServiceProvider, CancellationToken, Task> Startup { get; }

        Func<IServiceProvider, CancellationToken, Task> Cleanup { get; }
    }
}
