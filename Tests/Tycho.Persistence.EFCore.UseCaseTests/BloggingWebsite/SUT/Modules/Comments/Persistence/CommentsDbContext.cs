using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Comments.Persistence;

internal class CommentsDbContext : UseCaseDbContext<CommentsDbContext>
{
    public DbSet<Comment> Comments { get; set; } = null!;
}
