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
    /// Database schema name to use for Tycho tables.
    /// Return <c>null</c> to use the database default schema.
    /// </summary>
    public virtual string? Schema => null;

    /// <summary>
    /// Database table name to use for Tycho events inbox.
    /// </summary>
    public virtual string InboxTableName => $"{GetDbContextName()}Inbox";

    /// <summary>
    /// Database table name to use for Tycho events outbox.
    /// </summary>
    public virtual string OutboxTableName => $"{GetDbContextName()}Outbox";

    /// <inheritdoc/>
    public TychoDbContext() : base() { }

    /// <inheritdoc/>
    public TychoDbContext(DbContextOptions options) : base(options) { }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InboxEntry>()
                    .ToTable(InboxTableName, Schema);

        modelBuilder.Entity<OutboxEntry>()
                    .ToTable(OutboxTableName, Schema);
    }

    private string GetDbContextName()
    {
        string[] suffixesToTrim = ["DbContext", "Db", "Context"];

        string name = GetType().Name;
        foreach (string? suffix in suffixesToTrim)
        {
            name = name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) ? name[..^suffix.Length] : name;
        }

        return name;
    }
}
