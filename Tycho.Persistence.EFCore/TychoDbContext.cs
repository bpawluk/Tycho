using System;
using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Database context containing the entities required by Tycho.
/// </summary>
public abstract class TychoDbContext : DbContext
{
    /// <summary>
    /// The database schema name to use for Tycho tables. Return null to use the database default schema.
    /// </summary>
    public virtual string? Schema => null;

    /// <summary>
    /// The database table name to use for the Tycho events inbox.
    /// </summary>
    public virtual string InboxTableName => $"{GetDbContextName()}Inbox";

    /// <summary>
    /// The database table name to use for the Tycho events outbox.
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
