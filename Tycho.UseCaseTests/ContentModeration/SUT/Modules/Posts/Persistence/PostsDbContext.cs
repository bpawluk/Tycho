using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Posts.Persistence;

internal class PostsDbContext : TychoDbContext
{
    public DbSet<Post> Posts { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ContentModeration.Posts.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}