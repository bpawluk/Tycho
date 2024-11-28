using System;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Settings for Tycho outbox consumer
/// </summary>
public class OutboxConsumerSettings
{
    /// <summary>
    /// An instance of the default settings
    /// </summary>
    public static readonly OutboxConsumerSettings Default = new();

    /// <summary>
    /// Gets or sets the maximum message delivery count for the outbox consumer
    /// </summary>
    /// <value>The number of times a single message can be consumed in case of redeliveries</value>
    public uint MaxDeliveryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the processing state expiration time for the outbox consumer
    /// </summary>
    /// <value>The processing time after which it is considered failed and the message can be redelivered</value>
    public TimeSpan ProcessingStateExpiration { get; set; } = TimeSpan.FromMinutes(5);
}
