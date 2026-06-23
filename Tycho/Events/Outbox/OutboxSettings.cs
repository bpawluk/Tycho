using System;

namespace Tycho.Events.Outbox
{
    /// <summary>
    /// Settings for the Tycho outbox processor.
    /// </summary>
    public class OutboxSettings
    {
        /// <summary>
        /// Gets the default settings instance.
        /// </summary>
        public static OutboxSettings Default => new OutboxSettings();

        /// <summary>
        /// Gets or sets the concurrency limit for the outbox processor.
        /// </summary>
        /// <value>The maximum number of messages being processed in parallel.</value>
        public int ConcurrencyLimit { get; set; } = 10;

        /// <summary>
        /// Gets or sets the initial polling interval for the outbox processor.
        /// </summary>
        /// <value>The initial time interval between outbox processor iterations.</value>
        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromSeconds(0.1);

        /// <summary>
        /// Gets or sets the polling interval multiplier for the outbox processor.
        /// </summary>
        /// <value>The factor by which the polling interval increases when the outbox processor is idle.</value>
        public double PollingIntervalMultiplier { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the maximum polling interval for the outbox processor.
        /// </summary>
        /// <value>The maximum time interval between outbox processor iterations.</value>
        /// <remarks>
        /// Processing stops when the polling interval exceeds this value.
        /// </remarks>
        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets or sets the processing timeout for the outbox processor.
        /// </summary>
        /// <value>The maximum duration of processing a single outbox message.</value>
        public TimeSpan MessageProcessingTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
