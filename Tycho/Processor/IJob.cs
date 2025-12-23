using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal interface IJob
    {
        Task<bool> ExecuteAsync(CancellationToken cancellationToken);
    }
}
