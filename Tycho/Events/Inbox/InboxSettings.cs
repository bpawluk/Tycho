using System;

namespace Tycho.Events.Inbox
{
    /// <summary>
    /// Settings for the Tycho inbox processor.
    /// </summary>
    public class InboxSettings
    {
        /// <summary>
        /// Gets the default settings instance.
        /// </summary>
        public static InboxSettings Default => new InboxSettings();

        /// <summary>
        /// Gets or sets the concurrency limit for the inbox processor.
        /// </summary>
        /// <value>The maximum number of messages being processed in parallel.</value>
        public int ConcurrencyLimit { get; set; } = 10;

        /// <summary>
        /// Gets or sets the initial polling interval for the inbox processor.
        /// </summary>
        /// <value>The initial time interval between inbox processor iterations.</value>
        public TimeSpan InitialPollingInterval { get; set; } = TimeSpan.FromSeconds(0.1);

        /// <summary>
        /// Gets or sets the polling interval multiplier for the inbox processor.
        /// </summary>
        /// <value>The factor by which the polling interval increases when the inbox processor is idle.</value>
        public double PollingIntervalMultiplier { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the maximum polling interval for the inbox processor.
        /// </summary>
        /// <value>The maximum time interval between inbox processor iterations.</value>
        /// <remarks>
        /// Processing stops when the polling interval exceeds this value.
        /// </remarks>
        public TimeSpan MaxPollingInterval { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets or sets the processing timeout for the inbox processor.
        /// </summary>
        /// <value>The maximum duration of processing a single inbox message.</value>
        public TimeSpan MessageProcessingTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
