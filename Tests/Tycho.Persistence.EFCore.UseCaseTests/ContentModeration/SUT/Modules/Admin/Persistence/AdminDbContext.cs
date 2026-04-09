using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Domain;

namespace Tycho.UseCaseTests.ContentModeration.SUT.Modules.Admin.Persistence;

internal class AdminDbContext : TychoDbContext
{
    public DbSet<AdminAction> AdminActions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ContentModeration.Admin.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}