using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

/// <summary>
/// Database context with entities required by Tycho
/// </summary>
public class TychoDbContext : DbContext
{
    /// <inheritdoc/>
    public TychoDbContext() : base() { }

    /// <inheritdoc/>
    public TychoDbContext(DbContextOptions options) : base(options) { }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>();
    }
}