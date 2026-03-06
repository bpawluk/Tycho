using System;

namespace Tycho.Processor
{
    internal class JobProcessorSettings
    {
        public int ConcurrencyLimit { get; set; } = 100;

        public TimeSpan InitialInterval { get; set; } = TimeSpan.FromSeconds(1);

        public double IntervalMultiplier { get; set; } = 2;

        public TimeSpan MaxInterval { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan ScheduleProcessingTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public TimeSpan JobProcessingTimeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}