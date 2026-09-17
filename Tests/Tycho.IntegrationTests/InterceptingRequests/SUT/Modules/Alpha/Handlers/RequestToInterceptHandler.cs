using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha.Handlers;

internal sealed class RequestToInterceptHandler(IAlphaModuleParent parent) : IRequestHandler<RequestToIntercept>
{
    public async Task HandleAsync(RequestToIntercept requestData, CancellationToken cancellationToken)
    {
        requestData.Trace.Add("module-handler");
        await parent.ExecuteAsync(requestData, cancellationToken);
    }
}
