using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Alpha.Interceptors;

internal class RequestInterceptor :
    IRequestInterceptor<RequestToForward>,
    IRequestInterceptor<BetaInRequest>,
    IRequestInterceptor<AlphaOutRequest>,
    IRequestInterceptor<RequestWithResponseToForward, string>,
    IRequestInterceptor<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>,
    IRequestInterceptor<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>
{
    public Task ExecuteBefore(RequestToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(RequestWithResponseToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(RequestToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task<string> ExecuteAfter(RequestWithResponseToForward requestData, string response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }

    public Task<BetaInRequestWithResponse.Response> ExecuteAfter(BetaInRequestWithResponse requestData, BetaInRequestWithResponse.Response response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }

    public Task<AlphaOutRequestWithResponse.Response> ExecuteAfter(AlphaOutRequestWithResponse requestData, AlphaOutRequestWithResponse.Response response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }
}
