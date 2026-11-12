using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Ventilation.Persistence;

internal class VentilationDbContext : TestDbContext<VentilationDbContext>
{
    public DbSet<AirQualityReadingEntry> AirQualityReadings { get; set; } = null!;
}
