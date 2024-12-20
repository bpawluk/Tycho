﻿using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Domain;

namespace Tycho.UseCaseTests.OnlineStore.SUT.Modules.Basket.Persistence;

internal class BasketDbContext : TychoDbContext
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
            });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "OnlineStore.Basket.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    private static class BasketItemShadowProperties
    {
        public const string BasketId = "BasketId";
    }
}