namespace Tycho.Persistence.EFCore.UseCaseTests._Infrastructure;

internal abstract class UseCaseDbContext<TContext> : TychoDbContext
    where TContext : UseCaseDbContext<TContext>
{
    protected UseCaseDbContext() : base(UseCaseDatabases.Current.GetOptions(typeof(TContext))) { }
}
