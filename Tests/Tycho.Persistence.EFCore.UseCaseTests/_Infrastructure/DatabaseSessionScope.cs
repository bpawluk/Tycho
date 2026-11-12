namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal static class DatabaseSessionScope
{
    private static readonly AsyncLocal<DatabaseSession?> s_current = new();

    internal static DatabaseSession Current => s_current.Value ?? throw new InvalidOperationException("Database scope must be bound before constructing a DbContext.");

    internal static IDisposable Bind(DatabaseSession options)
    {
        DatabaseSession? previous = s_current.Value;
        s_current.Value = options;
        return new DatabaseBinding(previous);
    }

    private sealed class DatabaseBinding(DatabaseSession? previous) : IDisposable
    {
        public void Dispose() => s_current.Value = previous;
    }
}
