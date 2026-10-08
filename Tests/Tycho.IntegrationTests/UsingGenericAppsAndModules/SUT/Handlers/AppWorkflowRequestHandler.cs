using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Handlers;

internal sealed class AppWorkflowRequestHandler<TInput, TOutput> : IRequestHandler<AppWorkflowRequest, string>
{
    public Task<string> HandleAsync(AppWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Test = Passed in App<{typeof(TInput).Name}, {typeof(TOutput).Name}>");
    }
}
