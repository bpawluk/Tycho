using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;

internal class BasketDbContext : UseCaseDbContext<BasketDbContext>
{
    public DbSet<Domain.Basket> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Basket>().OwnsMany(
            basket => basket.Items,
            basketItem =>
            {
                basketItem.ToTable($"{typeof(BasketItem).Name}s");
                basketItem.WithOwner().HasForeignKey(BasketItemShadowProperties.BasketId);
                basketItem.HasKey(BasketItemShadowProperties.BasketId, nameof(BasketItem.ProductId));
                basketItem.Property(item => item.ProductId).ValueGeneratedNever();
            });
    }

    private static class BasketItemShadowProperties
    {
        public const string BasketId = "BasketId";
    }
}
