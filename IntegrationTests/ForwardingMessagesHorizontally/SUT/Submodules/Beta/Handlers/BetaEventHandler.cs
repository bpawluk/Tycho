using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta.Handlers;

internal class BetaEventHandler(IModule module) :
    IEventHandler<EventToForward>,
    IEventHandler<BetaInEvent>
{
    private readonly IModule _module = module;

    public Task Handle(EventToForward eventData, CancellationToken cancellationToken)
    {
        _module.Publish(eventData, cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(BetaInEvent eventData, CancellationToken cancellationToken)
    {
        _module.Publish<BetaOutEvent>(new(eventData.Result), cancellationToken);
        return Task.CompletedTask;
    }
}
