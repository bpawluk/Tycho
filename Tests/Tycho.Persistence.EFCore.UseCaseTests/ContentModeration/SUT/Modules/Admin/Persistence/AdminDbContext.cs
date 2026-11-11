using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;

internal class AdminDbContext : UseCaseDbContext<AdminDbContext>
{
    public DbSet<AdminAction> AdminActions { get; set; } = null!;
}
