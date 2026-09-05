using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal interface IJobFactory
    {
        Task<IJob?> TryCreateJobAsync(CancellationToken cancellationToken);
    }
}
