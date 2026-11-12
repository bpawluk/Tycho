using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha.Handlers;

internal sealed class RequestWithResponseToInterceptHandler(IAlphaModuleParent parent) : IRequestHandler<RequestWithResponseToIntercept, string>
{
    public async Task<string> HandleAsync(RequestWithResponseToIntercept requestData, CancellationToken cancellationToken)
    {
        requestData.Trace.Add("module-handler");
        return await parent.ExecuteAsync(requestData, cancellationToken);
    }
}
