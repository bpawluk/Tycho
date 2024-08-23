using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha.Handlers;

internal class AlphaEventHandler(IModule module) :
    IEventHandler<EventToForward>,
    IEventHandler<AlphaInEvent>
{
    private readonly IModule _module = module;

    public Task Handle(EventToForward eventData, CancellationToken cancellationToken)
    {
        _module.Publish(eventData, cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaInEvent eventData, CancellationToken cancellationToken)
    {
        _module.Publish<AlphaOutEvent>(new(eventData.Result), cancellationToken);
        return Task.CompletedTask;
    }
}
