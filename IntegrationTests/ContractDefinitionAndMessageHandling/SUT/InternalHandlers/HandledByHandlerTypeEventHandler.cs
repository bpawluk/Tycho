using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeEventHandler : IEventHandler<HandledByHandlerTypeEvent>
{
    private readonly IModule _thisModule;

    public HandledByHandlerTypeEventHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerTypeEvent eventData, CancellationToken cancellationToken)
    {
        _thisModule.Publish(eventData);
        return Task.CompletedTask;
    }
}
