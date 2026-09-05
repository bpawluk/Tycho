using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Handlers;

internal sealed class RequestToInterceptHandler : IRequestHandler<RequestToIntercept>
{
    public Task HandleAsync(RequestToIntercept requestData, CancellationToken cancellationToken)
    {
        requestData.Trace.Add("app-handler");
        return Task.CompletedTask;
    }
}
