using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;

internal class SecurityDbContext : TestDbContext<SecurityDbContext>
{
    public DbSet<SecurityEventEntry> SecurityEvents { get; set; } = null!;
}
