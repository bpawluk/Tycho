using MySql.Data.MySqlClient;
using Testcontainers.MsSql;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;
using Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

[assembly: AssemblyFixture(typeof(DatabaseServers))]

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

public sealed class DatabaseServers : IAsyncLifetime
{
    internal static bool MySqlDeadlockDiagnosticsEnabled =>
        Environment.GetEnvironmentVariable("TYCHO_MYSQL_DEADLOCK_DIAGNOSTICS") == "1";
    internal const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest@sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090";
    internal const string PostgreSqlImage = "postgres:17-bookworm@sha256:3645570cccdfa447589da9f57dd740faa29b30938e861289a5574b6ca6b03826";
    internal const string MySqlImage = "mysql:8.4@sha256:6ea90827b1100f8f2ae306a539f86d2c264a26ed435a2a9f75551dd5c3aeb242";

    private readonly SemaphoreSlim _gate = new(1);
    private MySqlContainer? _mySql;
    private string? _mySqlConnection;
    private MsSqlContainer? _sqlServer;
    private PostgreSqlContainer? _postgreSql;
    private string? _sqlConnection;
    private string? _postgresConnection;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    internal async Task<string> GetConnectionStringAsync(DatabaseProvider provider, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            using var startup = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            startup.CancelAfter(TimeSpan.FromMinutes(5));
            switch (provider)
            {
                case DatabaseProvider.SqlServer:
                    if (_sqlConnection is not null)
                        return _sqlConnection;
                    _sqlServer ??= new MsSqlBuilder(SqlServerImage).Build();
                    await _sqlServer.StartAsync(startup.Token);
                    return _sqlConnection = _sqlServer.GetConnectionString();
                case DatabaseProvider.PostgreSql:
                    if (_postgresConnection is not null)
                        return _postgresConnection;
                    _postgreSql ??= new PostgreSqlBuilder(PostgreSqlImage).Build();
                    await _postgreSql.StartAsync(startup.Token);
                    return _postgresConnection = _postgreSql.GetConnectionString();
                case DatabaseProvider.MySql:
                    if (_mySqlConnection is not null)
                        return _mySqlConnection;
                    // Root is confined to the disposable test container; scenarios
                    // create and delete separate databases for their module contexts.
                    var mySqlBuilder = new MySqlBuilder(MySqlImage).WithUsername("root");
                    if (MySqlDeadlockDiagnosticsEnabled)
                        mySqlBuilder = mySqlBuilder.WithCommand("--innodb-print-all-deadlocks=ON");
                    _mySql ??= mySqlBuilder.Build();
                    await _mySql.StartAsync(startup.Token);
                    return _mySqlConnection = _mySql.GetConnectionString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider));
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new InvalidOperationException(
                $"{provider} use-case tests require a reachable Linux Docker engine and permission to pull/start the pinned image. Infrastructure failures are not skipped.", exception);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_sqlServer is not null)
                await _sqlServer.DisposeAsync();
        }
        finally
        {
            try
            {
                if (_postgreSql is not null)
                    await _postgreSql.DisposeAsync();
            }
            finally
            {
                try
                {
                    if (_mySql is not null)
                        await _mySql.DisposeAsync();
                }
                finally
                {
                    _gate.Dispose();
                }
            }
        }
    }

    internal async Task<string?> GetMySqlDeadlockReportAsync()
    {
        if (!MySqlDeadlockDiagnosticsEnabled || _mySqlConnection is null)
            return null;
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var options = new MySqlConnectionStringBuilder(_mySqlConnection)
        {
            SslMode = MySqlSslMode.Disabled,
            Pooling = false,
            ConnectionTimeout = 10,
            DefaultCommandTimeout = 10
        };
        await using var connection = new MySqlConnection(options.ConnectionString);
        await connection.OpenAsync(timeout.Token);
        await using var command = new MySqlCommand("SHOW ENGINE INNODB STATUS", connection);
        await using var reader = await command.ExecuteReaderAsync(timeout.Token);
        if (!await reader.ReadAsync(timeout.Token))
            return null;
        string status = reader.GetString(2).Replace("\\n", "\n", StringComparison.Ordinal);
        int start = status.IndexOf("LATEST DETECTED DEADLOCK", StringComparison.Ordinal);
        if (start < 0)
            return null;
        int end = status.IndexOf("\nTRANSACTIONS\n", start, StringComparison.Ordinal);
        return end < 0 ? status[start..] : status[start..end];
    }
}
