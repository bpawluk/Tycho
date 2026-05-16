using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;

internal class FeedsDbContext : TychoDbContext
{
    public DbSet<Entry> Entries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Entry>()
                    .Property(entry => entry.Version)
                    .IsConcurrencyToken();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Feeds.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
