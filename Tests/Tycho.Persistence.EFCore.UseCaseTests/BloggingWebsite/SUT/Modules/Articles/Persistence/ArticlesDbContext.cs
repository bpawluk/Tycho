using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;

internal class ArticlesDbContext : TychoDbContext
{
    public DbSet<Article> Articles { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Articles.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
