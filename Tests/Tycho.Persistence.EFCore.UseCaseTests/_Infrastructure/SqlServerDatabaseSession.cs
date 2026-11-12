using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal sealed class SqlServerDatabaseSession(string serverConnectionString) : DatabaseSession
{
    protected override DbContextOptions CreateOptions(string databaseName)
    {
        var connection = new SqlConnectionStringBuilder(serverConnectionString)
        {
            InitialCatalog = databaseName,
            Pooling = false,
            ConnectTimeout = ConnectionTimeoutSeconds
        };
        return new DbContextOptionsBuilder()
            .UseSqlServer(
                connection.ConnectionString,
                options => options.CommandTimeout(CommandTimeoutSeconds))
            .Options;
    }
}
