using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;

internal sealed class MySqlDatabaseSession(string serverConnectionString) : DatabaseSession
{
    protected override DbContextOptions CreateOptions(string databaseName)
    {
        var connection = new MySqlConnectionStringBuilder(serverConnectionString)
        {
            Database = databaseName,
            Pooling = false,
            SslMode = MySqlSslMode.Disabled,
            ConnectionTimeout = ConnectionTimeoutSeconds,
            DefaultCommandTimeout = CommandTimeoutSeconds
        };
        return new DbContextOptionsBuilder()
            .UseMySQL(
                connection.ConnectionString,
                options => options.CommandTimeout(CommandTimeoutSeconds))
            .Options;
    }
}
