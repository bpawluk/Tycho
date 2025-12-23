using System;

namespace Tycho.Events.Inbox
{
    /// <summary>
    /// Settings for Tycho outbox processor
    /// </summary>
    public class InboxProcessorSettings
    {
        /// <summary>
        /// An instance of the default settings
        /// </summary>
        public static readonly InboxProcessorSettings Default = new InboxProcessorSettings();

        /// <summary>
        /// Gets or sets the batch size for the outbox processor
        /// </summary>
        /// <value>The number of messages to read from the outbox in each processor iteration</value>
        public int BatchSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets the initial polling interval for the outbox processor
        /// </summary>
        /// <value>The initial time interval between outbox processor iterations</value>
        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromSeconds(0.1);

        /// <summary>
        /// Gets or sets the maximum polling interval for the outbox processor
        /// </summary>
        /// <value>
        /// The maximum time interval between outbox processor iterations
        /// (processings stops when the polling interval exceeds this value)
        /// </value>
        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromSeconds(2.0);

        /// <summary>
        /// Gets or sets the polling interval multiplier for the outbox processor
        /// </summary>
        /// <value>The factor by which the polling interval increases when the outbox processor is idle</value>
        public double PollingIntervalMultiplier { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the processing timeout for the outbox processor
        /// </summary>
        /// <value>The maximum duration of a single outbox processor iteration</value>
        public TimeSpan ProcessingTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}