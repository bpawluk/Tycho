using System;

namespace Tycho.Persistence.EFCore.Retention;

/// <summary>
/// Defines how long completed messages and their payloads are retained.
/// </summary>
public sealed class MessageRetentionOptions
{
    /// <summary>
    /// Gets or sets the age after which completed message payloads are cleared. Null retains payloads until full cleanup.
    /// </summary>
    public TimeSpan? PayloadRetention { get; set; }

    /// <summary>
    /// Gets or sets the age after which completed messages are deleted. Null retains messages indefinitely.
    /// </summary>
    public TimeSpan? FullCleanupRetention { get; set; }

    internal bool IsEnabled => PayloadRetention.HasValue || FullCleanupRetention.HasValue;

    internal void Validate()
    {
        if (PayloadRetention <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(PayloadRetention),
                "Payload retention must be positive.");
        }

        if (FullCleanupRetention <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(FullCleanupRetention),
                "Full cleanup retention must be positive.");
        }

        if (PayloadRetention is TimeSpan payloadRetention &&
            FullCleanupRetention is TimeSpan fullCleanupRetention &&
            payloadRetention >= fullCleanupRetention)
        {
            throw new ArgumentException("Payload retention must be shorter than full cleanup retention.");
        }
    }

    internal MessageRetentionOptions Copy()
    {
        return new()
        {
            PayloadRetention = PayloadRetention,
            FullCleanupRetention = FullCleanupRetention
        };
    }
}
