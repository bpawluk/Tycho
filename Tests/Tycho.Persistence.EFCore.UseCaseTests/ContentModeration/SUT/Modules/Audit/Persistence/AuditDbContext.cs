using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Persistence;

internal class AuditDbContext : TychoDbContext
{
    public DbSet<AuditEntry> AuditEntries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ContentModeration.Audit.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
