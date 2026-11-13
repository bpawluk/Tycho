using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;
using Tycho.Persistence.EFCore.TestUtils.Databases;

namespace Tycho.Persistence.EFCore.IntegrationTests;

public abstract class DatabaseTest(DatabaseServers servers) : IAsyncLifetime
{
    private DatabaseSession? _session;
    private DbContextOptions? _options;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    protected async Task InitializeDatabaseAsync(DatabaseProvider provider)
    {
        _session = await DatabaseSessionFactory.CreateAsync(servers, provider);
        _options = _session.GetOptions(GetType());
    }

    protected DbContextOptionsBuilder<T> Options<T>() where T : DbContext =>
        new(new DbContextOptions<T>(_options!.Extensions.ToDictionary(e => e.GetType(), e => e)));

    public virtual async ValueTask DisposeAsync()
    {
        if (_session is not null)
            await _session.DisposeAsync();
    }
}
