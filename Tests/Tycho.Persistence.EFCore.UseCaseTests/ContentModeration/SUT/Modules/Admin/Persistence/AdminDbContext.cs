using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;

internal class AdminDbContext : TestDbContext<AdminDbContext>
{
    public DbSet<AdminAction> AdminActions { get; set; } = null!;
}
