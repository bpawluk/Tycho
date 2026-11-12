using System;

namespace Tycho.Persistence.EFCore.Inbox;

/// <summary>
/// Settings for the Tycho inbox consumer.
/// </summary>
public sealed class InboxConsumerSettings
{
    /// <summary>
    /// Gets a fresh instance of the default settings.
    /// </summary>
    public static InboxConsumerSettings Default => new();

    /// <summary>
    /// Gets or sets the maximum message processing count for the inbox consumer.
    /// </summary>
    /// <value>The number of times a single message can be processed including the first attempt.</value>
    public uint MaxProcessingCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the handling process expiration time for the inbox consumer.
    /// </summary>
    /// <value>The processing time after which it is considered failed and the message can be processed again.</value>
    /// <remarks>Configure this longer than the expected message processing duration.</remarks>
    public TimeSpan ProcessingExpiration { get; set; } = TimeSpan.FromMinutes(1);

    internal void Validate()
    {
        if (MaxProcessingCount == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxProcessingCount), "The attempt limit must include at least the first attempt.");
        }

        if (ProcessingExpiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ProcessingExpiration), "Claim expiration must be positive.");
        }
    }

    internal InboxConsumerSettings Copy()
    {
        return new()
        {
            MaxProcessingCount = MaxProcessingCount,
            ProcessingExpiration = ProcessingExpiration
        };
    }
}
