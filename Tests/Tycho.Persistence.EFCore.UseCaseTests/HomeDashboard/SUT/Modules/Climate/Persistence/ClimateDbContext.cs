using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;
using Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Modules.Climate.Persistence;

internal class ClimateDbContext : UseCaseDbContext<ClimateDbContext>
{
    public DbSet<TemperatureReadingEntry> TemperatureReadings { get; set; } = null!;
}
