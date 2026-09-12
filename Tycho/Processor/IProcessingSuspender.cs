using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal interface IProcessingSuspender
    {
        Task<SuspendResult> SuspendAsync(TimeSpan duration, CancellationToken cancellationToken);

        void TryResume();
    }
}
