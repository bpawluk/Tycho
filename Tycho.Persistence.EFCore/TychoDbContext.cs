using System;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Database context with entities required by Tycho
/// </summary>
public abstract class TychoDbContext : DbContext
{
    /// <summary>
    /// An identifier used for naming Inbox and Outbox tables.
    /// Must be unique for each Module instance to guarantee correct processing.
    /// </summary>
    public virtual string InboxAndOutboxIdentifier => GetDefaultInboxAndOutboxIdentifier();

    /// <inheritdoc/>
    public TychoDbContext() : base() { }

    /// <inheritdoc/>
    public TychoDbContext(DbContextOptions options) : base(options) { }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var inboxAndOutboxIdentifier = InboxAndOutboxIdentifier;
        if (string.IsNullOrWhiteSpace(inboxAndOutboxIdentifier))
        {
            throw new InvalidOperationException($"{nameof(InboxAndOutboxIdentifier)} must not be empty.");
        }

        modelBuilder.Entity<InboxMessage>()
                    .ToTable($"{inboxAndOutboxIdentifier}Inbox");

        modelBuilder.Entity<OutboxMessage>()
                    .ToTable($"{inboxAndOutboxIdentifier}Outbox");
    }

    private string GetDefaultInboxAndOutboxIdentifier()
    {
        var suffixesToTrim = new[] { "DbContext", "Db", "Context" };

        static string TrimSuffix(string name, string suffix)
        {
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                ? name[..^suffix.Length]
                : name;
        }

        var identifier = GetType().Name;
        foreach (var suffix in suffixesToTrim)
        {
            identifier = TrimSuffix(identifier, suffix);
        }

        return identifier;
    }
}
