using System;

namespace Tycho.Processor
{
    internal class JobProcessorSettings
    {
        public TimeSpan InitialInterval { get; set; }

        public double IntervalMultiplier { get; set; }

        public TimeSpan MaxInterval { get; set; }

        public TimeSpan ProcessingTimeout { get; set; }

        public JobProcessorSettings(
            TimeSpan initialInterval,
            double intervalMultiplier,
            TimeSpan maxInterval,
            TimeSpan processingTimeout)
        {
            InitialInterval = initialInterval;
            IntervalMultiplier = intervalMultiplier;
            MaxInterval = maxInterval;
            ProcessingTimeout = processingTimeout;
        }
    }
}