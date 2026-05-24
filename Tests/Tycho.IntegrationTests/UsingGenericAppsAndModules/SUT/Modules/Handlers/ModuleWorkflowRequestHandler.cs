using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules.Handlers;

internal sealed class ModuleWorkflowRequestHandler<TPayload, TKey>(TestModule<TPayload, TKey>.IParent parent)
    : IRequestHandler<ModuleWorkflowRequest, string>
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    private readonly TestModule<TPayload, TKey>.IParent _parent = parent;

    public Task<string> HandleAsync(ModuleWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new CompleteWorkflowRequest(requestData.Result), cancellationToken);
    }
}
