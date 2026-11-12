namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal static class DatabaseSessionFactory
{
    internal static async Task<DatabaseSession> CreateAsync(DatabaseServers servers, DatabaseProvider provider)
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DatabaseSession databases = provider switch
        {
            DatabaseProvider.SQLite => new SqliteDatabaseSession(),
            DatabaseProvider.SqlServer => new SqlServerDatabaseSession(await servers.GetConnectionStringAsync(provider, cancellationToken)),
            DatabaseProvider.PostgreSql => new PostgreSqlDatabaseSession(await servers.GetConnectionStringAsync(provider, cancellationToken)),
            DatabaseProvider.MySql => new MySqlDatabaseSession(await servers.GetConnectionStringAsync(provider, cancellationToken)),
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };

        TestContext.Current.TestOutputHelper?.WriteLine($"Database provider: {provider}; runtime: {Environment.Version}");
        return databases;
    }
}
