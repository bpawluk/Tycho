using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Handlers;

internal sealed class AppWorkflowRequestHandler<TPayload, TKey>(ITestModule<TPayload, TKey> module)
    : IRequestHandler<AppWorkflowRequest, string>
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    private readonly ITestModule<TPayload, TKey> _module = module;

    public Task<string> HandleAsync(AppWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        requestData.Result.LastHandledBy = "app";
        return _module.ExecuteAsync(new ModuleWorkflowRequest(requestData.Result), cancellationToken);
    }
}

internal sealed class CompleteWorkflowRequestHandler<TPayload, TKey>
    : IRequestHandler<CompleteWorkflowRequest, string>
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    public Task<string> HandleAsync(CompleteWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        requestData.Result.LastHandledBy = "module-parent-chain";
        return Task.FromResult("Test = Passed");
    }
}
