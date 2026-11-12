using System;

namespace Tycho.Persistence.EFCore.Outbox;

/// <summary>
/// Settings for the Tycho outbox consumer.
/// </summary>
public sealed class OutboxConsumerSettings
{
    /// <summary>
    /// Gets a fresh instance of the default settings.
    /// </summary>
    public static OutboxConsumerSettings Default => new();

    /// <summary>
    /// Gets or sets the maximum message delivery count for the outbox consumer.
    /// </summary>
    /// <value>The number of times a single message can be consumed including the first attempt.</value>
    public uint MaxDeliveryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delivery process expiration time for the outbox consumer.
    /// </summary>
    /// <value>The delivery time after which it is considered failed and the message can be redelivered.</value>
    /// <remarks>Configure this longer than the expected message processing duration.</remarks>
    public TimeSpan DeliveryExpiration { get; set; } = TimeSpan.FromMinutes(1);

    internal void Validate()
    {
        if (MaxDeliveryCount == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxDeliveryCount), "The attempt limit must include at least the first attempt.");
        }

        if (DeliveryExpiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(DeliveryExpiration), "Claim expiration must be positive.");
        }
    }

    internal OutboxConsumerSettings Copy()
    {
        return new()
        {
            MaxDeliveryCount = MaxDeliveryCount,
            DeliveryExpiration = DeliveryExpiration
        };
    }
}
