using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal interface IJobRunner : IDisposable
    {
        Task WaitForCapacityAsync(CancellationToken cancellationToken);

        void Run(IJob job);

        Task StopAsync();
    }
}
