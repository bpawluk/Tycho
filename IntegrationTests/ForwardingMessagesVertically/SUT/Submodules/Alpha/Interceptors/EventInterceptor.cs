using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Alpha.Interceptors;

internal class EventInterceptor :
    IEventInterceptor<EventToForward>,
    IEventInterceptor<BetaInEvent>,
    IEventInterceptor<AlphaOutEvent>
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

    public Task ExecuteBefore(AlphaOutEvent eventData, CancellationToken cancellationToken)
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

    public Task ExecuteAfter(AlphaOutEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.PostInterceptions++;
        return Task.CompletedTask;
    }
}
