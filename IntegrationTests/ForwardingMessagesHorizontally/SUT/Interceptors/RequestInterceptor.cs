using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta;
using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Interceptors;

internal class RequestInterceptor :
    IRequestInterceptor<RequestToForward>,
    IRequestInterceptor<BetaInRequest>,
    IRequestInterceptor<GammaInRequest>,
    IRequestInterceptor<RequestWithResponseToForward, string>,
    IRequestInterceptor<BetaInRequestWithResponse, BetaInRequestWithResponse.Response>,
    IRequestInterceptor<GammaInRequestWithResponse, GammaInRequestWithResponse.Response>
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

    public Task ExecuteBefore(GammaInRequest requestData, CancellationToken cancellationToken)
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

    public Task ExecuteBefore(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
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

    public Task ExecuteAfter(GammaInRequest requestData, CancellationToken cancellationToken)
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

    public Task<GammaInRequestWithResponse.Response> ExecuteAfter(GammaInRequestWithResponse requestData, GammaInRequestWithResponse.Response response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }
}
