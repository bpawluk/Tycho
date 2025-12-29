using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Ordering.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;

internal class OrderingDbContext : TychoDbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineStore.Ordering.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}