using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal static class TaskExtensions
    {
        public static async Task WaitWithCancellationAsync(this Task task, CancellationToken cancellationToken)
        {
            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
            {
                await task.ConfigureAwait(false);
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var cancellationCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            using CancellationTokenRegistration registration = cancellationToken.Register(
                state => ((TaskCompletionSource<bool>)state!).TrySetResult(true),
                cancellationCompletion);

            if (await Task.WhenAny(task, cancellationCompletion.Task).ConfigureAwait(false) != task)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            await task.ConfigureAwait(false);
        }
    }
}
