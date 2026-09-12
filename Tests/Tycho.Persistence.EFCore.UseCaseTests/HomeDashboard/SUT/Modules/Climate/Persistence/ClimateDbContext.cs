using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;

internal class ClimateDbContext : TychoDbContext
{
    public DbSet<TemperatureReadingEntry> TemperatureReadings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "HomeDashboard.Climate.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
