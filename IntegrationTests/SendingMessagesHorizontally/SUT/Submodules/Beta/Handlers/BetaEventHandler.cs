using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta.Handlers;

internal class BetaEventHandler(IModule module) :
    IEventHandler<EventToSend>,
    IEventHandler<BetaInEvent>
{
    private readonly IModule _module = module;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _module.Publish(eventData, cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(BetaInEvent eventData, CancellationToken cancellationToken)
    {
        eventData.Result.HandlingCount++;
        _module.Publish<BetaOutEvent>(new(eventData.Result), cancellationToken);
        return Task.CompletedTask;
    }
}
