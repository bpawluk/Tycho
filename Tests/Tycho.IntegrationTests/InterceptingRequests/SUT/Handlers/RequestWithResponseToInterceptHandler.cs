using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Handlers;

internal sealed class RequestWithResponseToInterceptHandler : IRequestHandler<RequestWithResponseToIntercept, string>
{
    public Task<string> HandleAsync(RequestWithResponseToIntercept requestData, CancellationToken cancellationToken)
    {
        requestData.Trace.Add("app-handler");
        return Task.FromResult("response");
    }
}
