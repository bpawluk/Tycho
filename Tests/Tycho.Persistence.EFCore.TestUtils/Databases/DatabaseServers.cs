using Testcontainers.MsSql;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Tycho.Persistence.EFCore.TestUtils.Databases;

public sealed class DatabaseServers : IAsyncLifetime
{
    internal const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest@sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090";
    internal const string PostgreSqlImage = "postgres:17-bookworm@sha256:3645570cccdfa447589da9f57dd740faa29b30938e861289a5574b6ca6b03826";
    internal const string MySqlImage = "mysql:8.4@sha256:6ea90827b1100f8f2ae306a539f86d2c264a26ed435a2a9f75551dd5c3aeb242";

    private static readonly TimeSpan s_startupTimeout = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _startupLock = new(1);

    private MsSqlContainer? _sqlServer;
    private string? _sqlServerConnectionString;

    private PostgreSqlContainer? _postgreSql;
    private string? _postgreSqlConnectionString;

    private MySqlContainer? _mySql;
    private string? _mySqlConnectionString;

    // The servers are started lazily in GetConnectionStringAsync to prevent starting containers that are not needed in a given test run.
    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    internal async Task<string> GetConnectionStringAsync(DatabaseProvider provider, CancellationToken cancellationToken)
    {
        await _startupLock.WaitAsync(cancellationToken);
        try
        {
            using var startup = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            startup.CancelAfter(s_startupTimeout);

            return provider switch
            {
                DatabaseProvider.SqlServer => await GetSqlServerConnectionStringAsync(startup.Token),
                DatabaseProvider.PostgreSql => await GetPostgreSqlConnectionStringAsync(startup.Token),
                DatabaseProvider.MySql => await GetMySqlConnectionStringAsync(startup.Token),
                _ => throw new ArgumentOutOfRangeException(nameof(provider))
            };
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new InvalidOperationException(
                $"{provider} tests require a reachable Linux Docker engine and permission to pull/start the pinned image.",
                exception);
        }
        finally
        {
            _startupLock.Release();
        }
    }

    private async Task<string> GetSqlServerConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (_sqlServerConnectionString is not null)
        {
            return _sqlServerConnectionString;
        }

        _sqlServer ??= new MsSqlBuilder(SqlServerImage).Build();
        await _sqlServer.StartAsync(cancellationToken);

        _sqlServerConnectionString = _sqlServer.GetConnectionString();
        return _sqlServerConnectionString;
    }

    private async Task<string> GetPostgreSqlConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (_postgreSqlConnectionString is not null)
        {
            return _postgreSqlConnectionString;
        }

        _postgreSql ??= new PostgreSqlBuilder(PostgreSqlImage).Build();
        await _postgreSql.StartAsync(cancellationToken);

        _postgreSqlConnectionString = _postgreSql.GetConnectionString();
        return _postgreSqlConnectionString;
    }

    private async Task<string> GetMySqlConnectionStringAsync(CancellationToken cancellationToken)
    {
        if (_mySqlConnectionString is not null)
        {
            return _mySqlConnectionString;
        }

        _mySql ??= new MySqlBuilder(MySqlImage).WithUsername("root").Build();
        await _mySql.StartAsync(cancellationToken);

        _mySqlConnectionString = _mySql.GetConnectionString();
        return _mySqlConnectionString;
    }

    public async ValueTask DisposeAsync()
    {
        var failures = new List<Exception>();
        IAsyncDisposable?[] containers = [_sqlServer, _postgreSql, _mySql];

        foreach (IAsyncDisposable? container in containers)
        {
            if (container is null)
            {
                continue;
            }

            try
            {
                await container.DisposeAsync();
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        }

        _startupLock.Dispose();

        if (failures.Count > 0)
        {
            throw new AggregateException("Database server cleanup failed.", failures);
        }
    }
}
