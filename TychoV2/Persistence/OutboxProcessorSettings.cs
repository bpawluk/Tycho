using System;

namespace TychoV2.Persistence
{
    internal class OutboxProcessorSettings
    {
        public int BatchSize { get; set; } = 5;

        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromSeconds(0.5);

        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromSeconds(5.0);

        public double PollingIntervalMultiplier { get; set; } = 2.0;
    }
}
