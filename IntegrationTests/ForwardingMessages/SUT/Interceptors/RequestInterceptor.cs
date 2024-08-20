using IntegrationTests.ForwardingMessages.SUT.Submodules;
using System.Threading.Tasks;
using System.Threading;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessages.SUT.Interceptors;

internal class RequestInterceptor :
    IRequestInterceptor<RequestToForward>,
    IRequestInterceptor<MappedAlphaRequest>,
    IRequestInterceptor<MappedBetaRequest>,
    IRequestInterceptor<MappedRequest>
{
    public Task ExecuteAfter(RequestToForward requestData, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedAlphaRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedBetaRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(RequestToForward requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedAlphaRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedBetaRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedRequest requestData, CancellationToken cancellationToken = default)
    {
        requestData.PreInterceptions++;
        return Task.CompletedTask;
    }
}
