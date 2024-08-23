using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta.Interceptors;

internal class RequestInterceptor :
    IRequestInterceptor<RequestToForward>,
    IRequestInterceptor<GammaInRequest>,
    IRequestInterceptor<BetaOutRequest>,
    IRequestInterceptor<RequestWithResponseToForward, string>,
    IRequestInterceptor<GammaInRequestWithResponse, GammaInRequestWithResponse.Response>,
    IRequestInterceptor<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>
{
    public Task ExecuteBefore(RequestToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(RequestWithResponseToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(RequestToForward requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task<string> ExecuteAfter(RequestWithResponseToForward requestData, string response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }

    public Task<GammaInRequestWithResponse.Response> ExecuteAfter(GammaInRequestWithResponse requestData, GammaInRequestWithResponse.Response response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }

    public Task<BetaOutRequestWithResponse.Response> ExecuteAfter(BetaOutRequestWithResponse requestData, BetaOutRequestWithResponse.Response response, CancellationToken cancellationToken)
    {
        requestData.Result.PostInterceptions++;
        return Task.FromResult(response);
    }
}
