using System.Security.Cryptography;
using System.Text;
using DotNet.Testcontainers.Containers;
using Testcontainers.MsSql;
using Testcontainers.MySql;
using Testcontainers.PostgreSql;

namespace Tycho.Persistence.EFCore.TestUtils.Databases;

public sealed class DatabaseServers : IDisposable
{
    internal const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest@sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090";
    internal const string PostgreSqlImage = "postgres:17-bookworm@sha256:3645570cccdfa447589da9f57dd740faa29b30938e861289a5574b6ca6b03826";
    internal const string MySqlImage = "mysql:8.4@sha256:6ea90827b1100f8f2ae306a539f86d2c264a26ed435a2a9f75551dd5c3aeb242";

    private const string ReuseLabel = "tycho.tests.database";
    private const string Password = "tycho.tests.password.01";
    private const string Version = "v1";

    private static readonly TimeSpan s_startupTimeout = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _startupLock = new(1);
    private readonly Dictionary<DatabaseProvider, IDatabaseContainer> _containers = [];

    internal async Task<string> GetConnectionStringAsync(DatabaseProvider provider, CancellationToken cancellationToken)
    {
        using var startup = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        startup.CancelAfter(s_startupTimeout);

        await _startupLock.WaitAsync(startup.Token);
        try
        {
            if (_containers.TryGetValue(provider, out IDatabaseContainer? existing))
            {
                return existing.GetConnectionString();
            }

            string name = GetContainerName(provider);
            using FileStream sharedStartupLock = await ContainerStartupLock.AcquireAsync(name, startup.Token);

            IDatabaseContainer container = BuildContainer(provider, name);
            await container.StartAsync(startup.Token);
            _containers.Add(provider, container);

            return container.GetConnectionString();
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

    private static string GetContainerName(DatabaseProvider provider)
    {
        string image = provider switch
        {
            DatabaseProvider.SqlServer => SqlServerImage,
            DatabaseProvider.PostgreSql => PostgreSqlImage,
            DatabaseProvider.MySql => MySqlImage,
            _ => throw new ArgumentOutOfRangeException(nameof(provider))
        };
        string imageHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(image)))[..12].ToLowerInvariant();
        return $"tycho-tests-{provider.ToString().ToLowerInvariant()}-{Version}-{imageHash}";
    }

    private static IDatabaseContainer BuildContainer(DatabaseProvider provider, string name) => provider switch
    {
        DatabaseProvider.SqlServer => new MsSqlBuilder(SqlServerImage)
            .WithDatabase("master")
            .WithPassword(Password)
            .WithName(name)
            .WithLabel(ReuseLabel, provider.ToString())
            .WithReuse(true)
            .Build(),
        DatabaseProvider.PostgreSql => new PostgreSqlBuilder(PostgreSqlImage)
            .WithDatabase("postgres")
            .WithUsername("postgres")
            .WithPassword(Password)
            .WithName(name)
            .WithLabel(ReuseLabel, provider.ToString())
            .WithReuse(true)
            .Build(),
        DatabaseProvider.MySql => new MySqlBuilder(MySqlImage)
            .WithDatabase("tycho")
            .WithUsername("root")
            .WithPassword(Password)
            .WithName(name)
            .WithLabel(ReuseLabel, provider.ToString())
            .WithReuse(true)
            .Build(),
        _ => throw new ArgumentOutOfRangeException(nameof(provider))
    };

    public void Dispose() => _startupLock.Dispose();
}
