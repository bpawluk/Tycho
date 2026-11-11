using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;

internal class SecurityDbContext : UseCaseDbContext<SecurityDbContext>
{
    public DbSet<SecurityEventEntry> SecurityEvents { get; set; } = null!;
}
