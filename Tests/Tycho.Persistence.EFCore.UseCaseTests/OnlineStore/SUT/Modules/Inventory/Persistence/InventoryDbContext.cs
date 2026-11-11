using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Inventory.Persistence;

internal class InventoryDbContext : UseCaseDbContext<InventoryDbContext>
{
    public DbSet<Item> Items { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<Item>().OwnsOne(item => item.Availability);

        modelBuilder.Entity<Item>().OwnsMany(
            item => item.Reservations,
            reservation =>
            {
                reservation.ToTable($"{typeof(Reservation).Name}s");
                reservation.WithOwner().HasForeignKey(ReservationShadowProperties.ItemId);
                reservation.HasKey(r => r.Code);
            });
    }

    private static class ReservationShadowProperties
    {
        public const string ItemId = "ItemId";
    }
}
