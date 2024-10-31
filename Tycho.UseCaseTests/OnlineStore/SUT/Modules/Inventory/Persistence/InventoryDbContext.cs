using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;

internal class InventoryDbContext : TychoDbContext
{
    public DbSet<Item> Items { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>().OwnsOne(product => product.StockLevel);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineStore.Inventory.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}