using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;

internal class ClimateDbContext : TestDbContext<ClimateDbContext>
{
    public DbSet<TemperatureReadingEntry> TemperatureReadings { get; set; } = null!;
}
