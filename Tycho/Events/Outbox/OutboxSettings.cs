using System;

namespace Tycho.Events.Outbox
{
    /// <summary>
    /// Settings for the Tycho outbox processor.
    /// </summary>
    public sealed class OutboxSettings
    {
        /// <summary>
        /// Gets the default settings instance.
        /// </summary>
        public static OutboxSettings Default => new();

        /// <summary>
        /// Gets or sets the concurrency limit for the outbox processor.
        /// </summary>
        /// <value>The maximum number of messages being processed in parallel.</value>
        public int ConcurrencyLimit { get; set; } = 5;

        /// <summary>
        /// Gets or sets the initial polling interval for the outbox processor.
        /// </summary>
        /// <value>The initial time interval between outbox processor iterations.</value>
        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Gets or sets the polling interval multiplier for the outbox processor.
        /// </summary>
        /// <value>The factor by which the polling interval increases when the outbox processor is idle.</value>
        public double PollingIntervalMultiplier { get; set; } = 3.0;

        /// <summary>
        /// Gets or sets the maximum polling interval for the outbox processor.
        /// </summary>
        /// <value>The maximum time interval between outbox processor iterations.</value>
        /// <remarks>
        /// Polling continues at this interval while the processor is idle.
        /// </remarks>
        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Gets or sets the processing timeout for the outbox processor.
        /// </summary>
        /// <value>The maximum duration of processing a single outbox message.</value>
        public TimeSpan MessageProcessingTimeout { get; set; } = TimeSpan.FromSeconds(5);

        internal void Validate()
        {
            if (ConcurrencyLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ConcurrencyLimit));
            }

            if (InitialPollingInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(InitialPollingInterval));
            }

            if (MaxPollingInterval < InitialPollingInterval)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxPollingInterval));
            }

            if (double.IsNaN(PollingIntervalMultiplier) || double.IsInfinity(PollingIntervalMultiplier) || PollingIntervalMultiplier <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(PollingIntervalMultiplier));
            }

            if (MessageProcessingTimeout <= TimeSpan.Zero && MessageProcessingTimeout != System.Threading.Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentOutOfRangeException(nameof(MessageProcessingTimeout));
            }
        }

        internal OutboxSettings Copy()
        {
            return new()
            {
                ConcurrencyLimit = ConcurrencyLimit,
                InitialPollingInterval = InitialPollingInterval,
                PollingIntervalMultiplier = PollingIntervalMultiplier,
                MaxPollingInterval = MaxPollingInterval,
                MessageProcessingTimeout = MessageProcessingTimeout
            };
        }
    }
}
