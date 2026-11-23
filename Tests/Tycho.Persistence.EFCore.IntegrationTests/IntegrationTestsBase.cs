namespace Tycho.Persistence.EFCore.IntegrationTests;

public abstract class IntegrationTestsBase(IntegrationTestsDatabaseBase database) : IAsyncLifetime
{
    private protected IntegrationTestsDatabaseBase Database { get; } = database;

    public async ValueTask InitializeAsync()
    {
        await Database.ResetAsync(TestContext.Current.CancellationToken);
        TestContext.Current.TestOutputHelper?.WriteLine(
            $"Database provider: {Database.Provider}; database: {Database.Name}; runtime: {Environment.Version}");
    }

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
