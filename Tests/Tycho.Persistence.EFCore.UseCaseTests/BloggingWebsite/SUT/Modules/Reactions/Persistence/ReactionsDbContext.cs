using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

internal class ReactionsDbContext : TychoDbContext
{
    public DbSet<Target> Targets { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Reactions.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}