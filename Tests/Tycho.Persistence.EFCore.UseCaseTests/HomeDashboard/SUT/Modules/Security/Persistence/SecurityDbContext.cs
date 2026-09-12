using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Security.Persistence;

internal class SecurityDbContext : TychoDbContext
{
    public DbSet<SecurityEventEntry> SecurityEvents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "HomeDashboard.Security.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
