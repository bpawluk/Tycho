using IntegrationTests.ForwardingMessages.SUT.Submodules;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessages.SUT.Interceptors;

internal class RequestWithResponseInterceptor :
    IRequestInterceptor<RequestWithResponseToForward, string>,
    IRequestInterceptor<MappedAlphaRequestWithResponse, AlphaResponse>,
    IRequestInterceptor<MappedBetaRequestWithResponse, BetaResponse>,
    IRequestInterceptor<MappedRequestWithResponse, string>
{
    public Task<string> ExecuteAfter(RequestWithResponseToForward requestData, string result, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<AlphaResponse> ExecuteAfter(MappedAlphaRequestWithResponse requestData, AlphaResponse result, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<BetaResponse> ExecuteAfter(MappedBetaRequestWithResponse requestData, BetaResponse result, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task<string> ExecuteAfter(MappedRequestWithResponse requestData, string result, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.FromResult(result);
    }

    public Task ExecuteBefore(RequestWithResponseToForward requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedAlphaRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedBetaRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }
}
