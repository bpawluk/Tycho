using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal interface IJobFactory
    {
        Task<IReadOnlyCollection<IJob>> CreateJobsAsync(int maxCount, CancellationToken cancellationToken);
    }
}
