using IntegrationTests.ForwardingMessages.SUT.Submodules;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Interceptors;

namespace IntegrationTests.ForwardingMessages.SUT.Interceptors;

internal class EventInterceptor : 
    IEventInterceptor<EventToForward>, 
    IEventInterceptor<MappedAlphaEvent>,
    IEventInterceptor<MappedBetaEvent>,
    IEventInterceptor<MappedEvent>
{
    public Task ExecuteAfter(EventToForward eventData, CancellationToken cancellationToken = default)
    {
        eventData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedAlphaEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedBetaEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteAfter(MappedEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PostInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(EventToForward eventData, CancellationToken cancellationToken = default)
    {
        eventData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedAlphaEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedBetaEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PreInterceptions++;
        return Task.CompletedTask;
    }

    public Task ExecuteBefore(MappedEvent eventData, CancellationToken cancellationToken = default)
    {
        eventData.PreInterceptions++;
        return Task.CompletedTask;
    }
}
