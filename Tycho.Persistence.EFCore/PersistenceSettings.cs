using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Defines settings for Tycho EF Core persistence.
/// </summary>
public sealed class PersistenceSettings
{
    /// <summary>
    /// Gets inbox consumer settings.
    /// </summary>
    public InboxConsumerSettings InboxConsumer { get; init; } = new();

    /// <summary>
    /// Gets outbox consumer settings.
    /// </summary>
    public OutboxConsumerSettings OutboxConsumer { get; init; } = new();

    internal void Validate()
    {
        InboxConsumer.Validate();
        OutboxConsumer.Validate();
    }

    internal PersistenceSettings Copy()
    {
        return new()
        {
            InboxConsumer = InboxConsumer.Copy(),
            OutboxConsumer = OutboxConsumer.Copy()
        };
    }
}
