using System;

namespace Tycho.Persistence.EFCore.Retention;

/// <summary>
/// Controls automatic cleanup of completed inbox and outbox entries.
/// </summary>
public sealed class PersistenceRetentionSettings
{
    /// <summary>
    /// Gets or sets the delay before the first cleanup. Zero runs immediately.
    /// </summary>
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the delay between cleanup runs.
    /// </summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets inbox retention settings.
    /// </summary>
    public MessageRetentionSettings Inbox { get; init; } = new() { PayloadRetention = TimeSpan.FromDays(7) };

    /// <summary>
    /// Gets outbox retention settings.
    /// </summary>
    public MessageRetentionSettings Outbox { get; init; } = new() { FullCleanupRetention = TimeSpan.FromDays(7) };

    internal bool IsRetentionEnabled => Inbox.IsEnabled || Outbox.IsEnabled;

    internal void Validate()
    {
        if (InitialDelay < TimeSpan.Zero || InitialDelay > TimeSpan.FromMilliseconds(uint.MaxValue - 1))
        {
            throw new ArgumentOutOfRangeException(nameof(InitialDelay),
                "Initial delay must be nonnegative and within the supported timer range.");
        }

        if (CleanupInterval <= TimeSpan.Zero || CleanupInterval > TimeSpan.FromMilliseconds(uint.MaxValue - 1))
        {
            throw new ArgumentOutOfRangeException(
                nameof(CleanupInterval),
                "Cleanup interval must be greater than zero and within the supported timer range.");
        }

        Inbox.Validate();
        Outbox.Validate();
    }

    internal PersistenceRetentionSettings Copy()
    {
        return new()
        {
            InitialDelay = InitialDelay,
            CleanupInterval = CleanupInterval,
            Inbox = Inbox.Copy(),
            Outbox = Outbox.Copy()
        };
    }
}
