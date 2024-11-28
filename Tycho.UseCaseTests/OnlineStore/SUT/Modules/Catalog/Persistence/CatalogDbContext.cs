using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;

internal class CatalogDbContext : TychoDbContext
{
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().OwnsOne(product => product.Availability);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineStore.Catalog.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}