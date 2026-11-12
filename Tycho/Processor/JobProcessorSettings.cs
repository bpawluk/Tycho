using System;

namespace Tycho.Processor
{
    internal sealed class JobProcessorSettings
    {
        public int ConcurrencyLimit { get; set; }

        public TimeSpan JobProcessingTimeout { get; set; }

        public TimeSpan InitialInterval { get; set; }

        public double IntervalMultiplier { get; set; }

        public TimeSpan MaxInterval { get; set; }

    }
}
