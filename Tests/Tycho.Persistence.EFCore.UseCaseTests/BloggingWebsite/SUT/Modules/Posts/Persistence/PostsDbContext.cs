using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Posts.Persistence;

internal class PostsDbContext : TestDbContext<PostsDbContext>
{
    public DbSet<Post> Posts { get; set; } = null!;
}
