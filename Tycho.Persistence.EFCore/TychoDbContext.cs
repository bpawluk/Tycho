using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tycho.Persistence.EFCore.Database;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.Transactions;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Database context containing the entities required by Tycho.
/// </summary>
public abstract class TychoDbContext : DbContext
{
    private static bool ShouldUseStateUpdatedIndex => true;
    private bool ShouldUseClaimIdIndex => Database.ProviderName is not (Providers.SqlServer or Providers.PostgreSql);

    internal TransactionInterceptor TransactionInterceptor { get; } = new();

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
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(TransactionInterceptor);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        EntityTypeBuilder<InboxEntry> inbox = modelBuilder.Entity<InboxEntry>().ToTable(InboxTableName, Schema);
        EntityTypeBuilder<OutboxEntry> outbox = modelBuilder.Entity<OutboxEntry>().ToTable(OutboxTableName, Schema);

        inbox.HasKey(entry => new { entry.OwnerId, entry.EntryId });
        outbox.HasKey(entry => new { entry.OwnerId, entry.EntryId });

        if (ShouldUseClaimIdIndex)
        {
            inbox.HasIndex(entry => new { entry.OwnerId, entry.ClaimId });
            outbox.HasIndex(entry => new { entry.OwnerId, entry.ClaimId });
        }

        if (ShouldUseStateUpdatedIndex)
        {
            inbox.HasIndex(entry => new { entry.OwnerId, entry.State, entry.Updated });
            outbox.HasIndex(entry => new { entry.OwnerId, entry.State, entry.Updated });
        }
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
