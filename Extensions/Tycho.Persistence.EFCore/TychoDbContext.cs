using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

/// <summary>
///     TODO
/// </summary>
public class TychoDbContext : DbContext
{
    public TychoDbContext() : base() { }

    public TychoDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>();
    }
}