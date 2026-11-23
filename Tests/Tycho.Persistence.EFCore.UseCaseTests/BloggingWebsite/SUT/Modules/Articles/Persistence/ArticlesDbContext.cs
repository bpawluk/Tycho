using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Articles.Persistence;

internal class ArticlesDbContext : TestDbContext<ArticlesDbContext>
{
    public DbSet<Article> Articles { get; set; } = null!;
}
