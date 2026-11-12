using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;

public abstract class DatabaseSession : IAsyncDisposable
{
    protected const int CommandTimeoutSeconds = 30;
    protected const int ConnectionTimeoutSeconds = 15;

    private static readonly TimeSpan s_cleanupTimeout = TimeSpan.FromSeconds(30);
    private readonly ConcurrentDictionary<Type, DbContextOptions> _optionsByContext = new();

    internal DbContextOptions GetOptions(Type contextType) => _optionsByContext.GetOrAdd(contextType, _ => CreateOptions($"Tycho_test_{Guid.NewGuid():N}"));

    protected abstract DbContextOptions CreateOptions(string databaseName);

    protected virtual void Cleanup() { }

    public async ValueTask DisposeAsync()
    {
        var failures = new List<Exception>();
        foreach (DbContextOptions dbContextOptions in _optionsByContext.Values)
        {
            try
            {
                using var cleanup = new CancellationTokenSource(s_cleanupTimeout);
                await using var context = new DbContext(dbContextOptions);
                await context.Database.EnsureDeletedAsync(cleanup.Token);
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        }

        try
        {
            Cleanup();
        }
        catch (Exception exception)
        {
            failures.Add(exception);
        }

        if (failures.Count > 0)
        {
            throw new AggregateException("Database cleanup failed.", failures);
        }

        GC.SuppressFinalize(this);
    }
}
