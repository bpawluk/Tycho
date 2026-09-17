using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Structure
{
    /// <summary>
    /// Represents a component controlled through an asynchronous start and stop lifecycle.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Starts the runnable component.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the runnable component.
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
