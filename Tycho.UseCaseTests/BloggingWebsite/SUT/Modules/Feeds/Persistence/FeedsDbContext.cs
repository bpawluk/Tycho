using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Feeds.Persistence;

internal class FeedsDbContext : TychoDbContext
{
    public DbSet<Entry> Entries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Feeds.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}