using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.BloggingWebsite.SUT.Modules.Reactions.Persistence;

internal class ReactionsDbContext : TestDbContext<ReactionsDbContext>
{
    public DbSet<Target> Targets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Target>().Property(target => target.Id).ValueGeneratedNever();
    }
}
