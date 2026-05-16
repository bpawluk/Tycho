using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore;
using Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Domain;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Users.Persistence;

internal class UsersDbContext : TychoDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "ContentModeration.Users.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
