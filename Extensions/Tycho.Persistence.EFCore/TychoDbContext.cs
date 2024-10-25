using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore;

public class TychoDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
