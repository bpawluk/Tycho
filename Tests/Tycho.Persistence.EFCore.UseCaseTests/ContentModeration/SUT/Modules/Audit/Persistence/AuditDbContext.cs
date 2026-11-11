using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Persistence;

internal class AuditDbContext : UseCaseDbContext<AuditDbContext>
{
    public DbSet<AuditEntry> AuditEntries { get; set; } = null!;
}
