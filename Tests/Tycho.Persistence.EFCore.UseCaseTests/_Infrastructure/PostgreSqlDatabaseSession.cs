using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal sealed class PostgreSqlDatabaseSession(string serverConnectionString) : DatabaseSession
{
    protected override DbContextOptions CreateOptions(string databaseName)
    {
        var connection = new NpgsqlConnectionStringBuilder(serverConnectionString)
        {
            Database = databaseName,
            Pooling = false,
            Timeout = ConnectionTimeoutSeconds,
            CommandTimeout = CommandTimeoutSeconds
        };
        return new DbContextOptionsBuilder().UseNpgsql(connection.ConnectionString).Options;
    }
}
