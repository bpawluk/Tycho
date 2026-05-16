using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;

internal class PostsDbContext : TychoDbContext
{
    public DbSet<Post> Posts { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Posts.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
