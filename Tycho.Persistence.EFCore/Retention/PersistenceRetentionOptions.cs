using System;

namespace Tycho.Persistence.EFCore.Retention;

/// <summary>
/// Controls automatic cleanup of completed inbox and outbox entries.
/// </summary>
public sealed class PersistenceRetentionOptions
{
    /// <summary>
    /// Gets or sets the delay between cleanup runs.
    /// </summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets inbox retention settings.
    /// </summary>
    public MessageRetentionOptions Inbox { get; } = new() { PayloadRetention = TimeSpan.FromDays(7) };

    /// <summary>
    /// Gets outbox retention settings.
    /// </summary>
    public MessageRetentionOptions Outbox { get; } = new() { FullCleanupRetention = TimeSpan.FromDays(7) };

    internal bool IsEnabled => Inbox.IsEnabled || Outbox.IsEnabled;

    internal void Validate()
    {
        if (CleanupInterval <= TimeSpan.Zero || CleanupInterval > TimeSpan.FromMilliseconds(uint.MaxValue - 1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(CleanupInterval),
                "Cleanup interval must be greater than zero and within the supported timer range.");
        }

        Inbox.Validate();
        Outbox.Validate();
    }
}
