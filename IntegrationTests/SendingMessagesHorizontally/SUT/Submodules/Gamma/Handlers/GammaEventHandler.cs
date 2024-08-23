using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma.Handlers;

internal class GammaEventHandler(IModule module) :
    IEventHandler<EventToSend>,
    IEventHandler<GammaInEvent>
{
    private readonly IModule _module = module;

    public Task Handle(EventToSend eventData, CancellationToken cancellationToken)
    {
        _module.Publish(eventData, cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(GammaInEvent eventData, CancellationToken cancellationToken)
    {
        _module.Publish<GammaOutEvent>(new(eventData.Result), cancellationToken);
        return Task.CompletedTask;
    }
}
