using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore;

public class TychoDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<OutboxMessage>();
    }
}