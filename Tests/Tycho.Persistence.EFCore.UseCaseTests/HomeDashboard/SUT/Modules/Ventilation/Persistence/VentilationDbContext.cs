using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Persistence;

internal class VentilationDbContext : TychoDbContext
{
    public DbSet<AirQualityReadingEntry> AirQualityReadings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "HomeDashboard.Ventilation.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
