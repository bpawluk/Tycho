using Microsoft.EntityFrameworkCore;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;
using Tycho.Persistence.EFCore.TestUtils.Databases;
using Tycho.Persistence.EFCore.TestUtils.Databases.Sessions;

namespace Tycho.Persistence.EFCore.IntegrationTests;

public abstract class IntegrationTestsDatabaseBase(DatabaseServers servers, DatabaseProvider provider) : IAsyncLifetime
{
    private DatabaseSession? _session;
    private DbContextOptions _options = null!;

    internal DatabaseProvider Provider { get; } = provider;
    internal string Name { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _session = await DatabaseSessionFactory.CreateAsync(servers, Provider);
        try
        {
            _options = _session.GetOptions(typeof(IntegrationTestsDbContext));
            await using var context = new IntegrationTestsDbContext(_options);
            await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
            Name = context.Database.IsSqlite() ? context.Database.GetDbConnection().DataSource : context.Database.GetDbConnection().Database;
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    internal DbContextOptionsBuilder<TContext> CreateOptionsBuilder<TContext>() where TContext : DbContext
    {
        var extensions = _options.Extensions.ToDictionary(extension => extension.GetType(), extension => extension);
        var options = new DbContextOptions<TContext>(extensions);
        return new(options);
    }

    internal async Task ResetAsync(CancellationToken cancellationToken)
    {
        await using var context = new IntegrationTestsDbContext(_options);
        await context.Set<InboxEntry>().ExecuteDeleteAsync(cancellationToken);
        await context.Set<OutboxEntry>().ExecuteDeleteAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_session is not null)
        {
            await _session.DisposeAsync();
            _session = null;
        }
        GC.SuppressFinalize(this);
    }
}
