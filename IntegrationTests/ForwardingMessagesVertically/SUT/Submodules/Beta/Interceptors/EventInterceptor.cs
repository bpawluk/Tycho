using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Gamma;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta.Interceptors;

internal class EventInterceptor :
    IEventInterceptor<EventToForward>,
    IEventInterceptor<GammaInEvent>,
    IEventInterceptor<BetaOutEvent>
{
    public Task ExecuteBefore(EventToForward eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(GammaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(BetaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(EventToForward eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(GammaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(BetaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }
}
