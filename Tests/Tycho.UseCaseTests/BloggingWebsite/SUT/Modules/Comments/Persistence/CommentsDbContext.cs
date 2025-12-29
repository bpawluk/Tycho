using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Domain;

namespace Tycho.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;

internal class CommentsDbContext : TychoDbContext
{
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "BloggingWebsite.Comments.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}