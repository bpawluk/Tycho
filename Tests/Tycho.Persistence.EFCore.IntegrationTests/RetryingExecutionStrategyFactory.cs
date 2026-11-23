using Microsoft.EntityFrameworkCore.Storage;

namespace Tycho.Persistence.EFCore.IntegrationTests;

internal sealed class RetryingExecutionStrategyFactory(ExecutionStrategyDependencies dependencies) : IExecutionStrategyFactory
{
    public IExecutionStrategy Create() => new RetryingStrategy(dependencies);

    private sealed class RetryingStrategy(ExecutionStrategyDependencies dependencies) : ExecutionStrategy(dependencies, 1, TimeSpan.Zero)
    {
        protected override bool ShouldRetryOn(Exception exception) => false;
    }
}
