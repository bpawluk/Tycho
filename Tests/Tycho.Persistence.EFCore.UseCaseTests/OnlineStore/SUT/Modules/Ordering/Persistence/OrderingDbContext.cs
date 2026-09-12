using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Persistence;

internal class OrderingDbContext : TychoDbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineStore.Ordering.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
