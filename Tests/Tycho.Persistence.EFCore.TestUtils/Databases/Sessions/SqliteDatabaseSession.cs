using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;

internal sealed class SqliteDatabaseSession : DatabaseSession
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"Tycho_test_{Guid.NewGuid():N}");

    internal SqliteDatabaseSession()
    {
        Directory.CreateDirectory(_directory);
    }

    protected override DbContextOptions CreateOptions(string databaseName)
    {
        string databasePath = Path.Combine(_directory, databaseName + ".db");
        return new DbContextOptionsBuilder()
            .UseSqlite($"Data Source={databasePath};Pooling=False",
                options => options.CommandTimeout(CommandTimeoutSeconds))
            .Options;
    }

    protected override void Cleanup()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
