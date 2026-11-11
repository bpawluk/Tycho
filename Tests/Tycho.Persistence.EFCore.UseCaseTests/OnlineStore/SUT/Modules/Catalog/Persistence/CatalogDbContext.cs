using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Catalog.Persistence;

internal class CatalogDbContext : UseCaseDbContext<CatalogDbContext>
{
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().OwnsOne(product => product.Availability);
    }
}
