namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal abstract class TestDbContext<TContext> : TychoDbContext
    where TContext : TestDbContext<TContext>
{
    protected TestDbContext() : base(DatabaseSessionScope.Current.GetOptions(typeof(TContext)))
    {
    }
}
