using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.UseCaseTests.HomeDashboard.SUT.Persistence;

internal class HomeDashboardDbContext : TychoDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "HomeDashboard.App.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
