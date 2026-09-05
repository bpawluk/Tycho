using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal sealed class ProcessingSuspender : IProcessingSuspender
    {
        private CancellationTokenSource? _currentPauseCts;

        public async Task<SuspendResult> SuspendAsync(TimeSpan duration, CancellationToken cancellationToken)
        {
            var currentPauseCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (Interlocked.CompareExchange(ref _currentPauseCts, currentPauseCts, null) != null)
            {
                currentPauseCts.Dispose();
                throw new InvalidOperationException("Only one suspension can be active at a time.");
            }

            try
            {
                await Task.Delay(duration, currentPauseCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Propagate cancellation from the caller, but swallow cancellation caused by resuming.
                cancellationToken.ThrowIfCancellationRequested();
                return SuspendResult.Interrupted;
            }
            finally
            {
                Interlocked.CompareExchange(ref _currentPauseCts, null, currentPauseCts);
                currentPauseCts.Dispose();
            }

            return SuspendResult.Completed;
        }

        public void TryResume()
        {
            CancellationTokenSource? currentPauseCts = Volatile.Read(ref _currentPauseCts);
            try
            {
                currentPauseCts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // The token was disposed between Volatile.Read and Cancel().
            }
        }
    }
}
