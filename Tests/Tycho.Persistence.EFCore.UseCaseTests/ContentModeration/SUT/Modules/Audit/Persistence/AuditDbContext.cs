using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Persistence;

internal class AuditDbContext : TestDbContext<AuditDbContext>
{
    public DbSet<AuditEntry> AuditEntries { get; set; } = null!;
}
