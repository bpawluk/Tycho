using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta;
using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Interceptors;

internal class EventInterceptor :
    IEventInterceptor<EventToForward>,
    IEventInterceptor<BetaInEvent>,
    IEventInterceptor<GammaInEvent>
{
    public Task ExecuteBefore(EventToForward eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(GammaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(EventToForward eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(BetaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(GammaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }
}
