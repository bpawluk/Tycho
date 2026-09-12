using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules.Handlers;

internal sealed class ModuleWorkflowRequestHandler<TInput, TOutput> : IRequestHandler<ModuleWorkflowRequest, string>
{
    public Task<string> HandleAsync(ModuleWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Test = Passed in Module<{typeof(TInput).Name}, {typeof(TOutput).Name}>");
    }
}
