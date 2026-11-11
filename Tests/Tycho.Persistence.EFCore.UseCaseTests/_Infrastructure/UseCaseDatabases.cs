using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal sealed class UseCaseDatabases(DatabaseServers servers, DatabaseProvider provider) : IAsyncDisposable
{
    private static readonly AsyncLocal<UseCaseDatabases?> Ambient = new();
    private readonly ConcurrentDictionary<Type, DbContextOptions> _options = new();
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"tycho_usecase_{Guid.NewGuid():N}");
    private readonly SqlCapture _sql = new();
    internal UseCaseLogCapture Logs { get; } = new();
    private string? _serverConnection;

    internal static UseCaseDatabases Current => Ambient.Value
        ?? throw new InvalidOperationException("A use-case database scope must be bound before constructing a DbContext.");

    internal async Task InitializeAsync()
    {
        if (provider == DatabaseProvider.SQLite)
            Directory.CreateDirectory(_directory);
        else
            _serverConnection = await servers.GetConnectionStringAsync(provider, TestContext.Current.CancellationToken);
        TestContext.Current.TestOutputHelper?.WriteLine($"Database provider: {provider}; runtime: {Environment.Version}");
        if (provider == DatabaseProvider.MySql)
            TestContext.Current.TestOutputHelper?.WriteLine($"MySQL EF provider: {typeof(MySQLDbContextOptionsExtensions).Assembly.GetName()}");
    }

    // The scope flows through async module startup and background workers, while
    // parallel scenarios keep separate values. Contexts capture it at construction.
    internal IDisposable Bind()
    {
        UseCaseDatabases? previous = Ambient.Value;
        Ambient.Value = this;
        return new Binding(previous);
    }

    internal DbContextOptions GetOptions(Type contextType) => _options.GetOrAdd(contextType, _ =>
    {
        string database = $"tycho_usecase_{Guid.NewGuid():N}";
        var builder = new DbContextOptionsBuilder();
        switch (provider)
        {
            case DatabaseProvider.SQLite:
                builder.UseSqlite($"Data Source={Path.Combine(_directory, database + ".db")};Pooling=False", options => options.CommandTimeout(30));
                break;
            case DatabaseProvider.SqlServer:
                var sql = new SqlConnectionStringBuilder(_serverConnection)
                {
                    InitialCatalog = database,
                    Pooling = false,
                    ConnectTimeout = 15
                };
                builder.UseSqlServer(sql.ConnectionString, options => options.CommandTimeout(30));
                break;
            case DatabaseProvider.PostgreSql:
                var postgres = new NpgsqlConnectionStringBuilder(_serverConnection)
                {
                    Database = database,
                    Pooling = false,
                    Timeout = 15,
                    CommandTimeout = 30
                };
                builder.UseNpgsql(postgres.ConnectionString);
                break;
            case DatabaseProvider.MySql:
                var mysql = new MySqlConnectionStringBuilder(_serverConnection
                    ?? throw new InvalidOperationException("MySQL server has not been initialized."))
                {
                    Database = database,
                    Pooling = false,
                    // Disposable local container only: Oracle's SSL authentication cancellation
                    // callback can throw IOException during worker shutdown. TLS is outside this suite.
                    SslMode = MySqlSslMode.Disabled,
                    ConnectionTimeout = 15,
                    DefaultCommandTimeout = 30
                };
                builder.UseMySQL(mysql.ConnectionString, options => options.CommandTimeout(30));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(provider));
        }
        return builder.AddInterceptors(_sql).Options;
    });

    public async ValueTask DisposeAsync()
    {
        var failures = new List<Exception>();
        if (provider == DatabaseProvider.MySql && DatabaseServers.MySqlDeadlockDiagnosticsEnabled)
        {
            try
            {
                string? report = await servers.GetMySqlDeadlockReportAsync();
                if (report is not null)
                    TestContext.Current.TestOutputHelper?.WriteLine($"MySQL server's latest deadlock (shared across scenarios):{Environment.NewLine}{report}");
            }
            catch (Exception exception)
            {
                TestContext.Current.TestOutputHelper?.WriteLine($"Could not capture MySQL deadlock diagnostics: {exception.Message}");
            }
        }
        foreach (DbContextOptions options in _options.Values)
        {
            try
            {
                // Cleanup has its own deadline even if the test was cancelled.
                using var cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(45));
                await using var context = new DbContext(options);
                await context.Database.EnsureDeletedAsync(cleanup.Token);
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        }
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
        foreach (string command in _sql.Commands)
            TestContext.Current.TestOutputHelper?.WriteLine(command);
        foreach (string failure in _sql.Failures)
            TestContext.Current.TestOutputHelper?.WriteLine($"Database command failed: {failure}");
        foreach (string message in Logs.Messages)
            TestContext.Current.TestOutputHelper?.WriteLine($"Application log: {message}");
        foreach (string plan in _sql.Plans)
            TestContext.Current.TestOutputHelper?.WriteLine($"MySQL transition query plan: {plan}");
        if (failures.Count > 0)
            throw new AggregateException("Use-case database cleanup failed.", failures);
    }

    private sealed class Binding(UseCaseDatabases? previous) : IDisposable
    {
        public void Dispose() => Ambient.Value = previous;
    }
}
