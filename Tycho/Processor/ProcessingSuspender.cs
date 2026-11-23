using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Processor
{
    internal sealed class ProcessingSuspender : IProcessingSuspender
    {
        private readonly SemaphoreSlim _wakeSignal = new(0, 1);
        private readonly object _resumeLock = new();
        private int _suspensionActive;

        public async Task<SuspendResult> SuspendAsync(TimeSpan duration, CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _suspensionActive, 1, 0) != 0)
            {
                throw new InvalidOperationException("Only one suspension can be active at a time.");
            }

            try
            {
                bool signaled = await _wakeSignal.WaitAsync(duration, cancellationToken).ConfigureAwait(false);
                return signaled ? SuspendResult.Interrupted : SuspendResult.Completed;
            }
            finally
            {
                Volatile.Write(ref _suspensionActive, 0);
            }
        }

        public void TryResume()
        {
            lock (_resumeLock)
            {
                if (_wakeSignal.CurrentCount == 0)
                {
                    _wakeSignal.Release();
                }
            }
        }
    }
}
