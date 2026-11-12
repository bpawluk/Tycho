namespace Tycho.Persistence.EFCore.TestUtils.Databases;

public abstract class TestDbContext<TContext> : TychoDbContext
    where TContext : TestDbContext<TContext>
{
    protected TestDbContext() : base(DatabaseSessionScope.Current.GetOptions(typeof(TContext)))
    {
    }
}
