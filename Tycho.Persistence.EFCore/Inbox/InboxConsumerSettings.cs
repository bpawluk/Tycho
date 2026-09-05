using System;

namespace Tycho.Persistence.EFCore.Inbox;

/// <summary>
/// Settings for the Tycho inbox consumer.
/// </summary>
public class InboxConsumerSettings
{
    /// <summary>
    /// An instance of the default settings.
    /// </summary>
    public static readonly InboxConsumerSettings Default = new();

    /// <summary>
    /// Gets or sets the maximum message processing count for the inbox consumer.
    /// </summary>
    /// <value>The number of times a single message can be processed in case of retries.</value>
    public uint MaxProcessingCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the handling process expiration time for the inbox consumer.
    /// </summary>
    /// <value>The processing time after which it is considered failed and the message can be processed again.</value>
    public TimeSpan ProcessingExpiration { get; set; } = TimeSpan.FromMinutes(1);
}
