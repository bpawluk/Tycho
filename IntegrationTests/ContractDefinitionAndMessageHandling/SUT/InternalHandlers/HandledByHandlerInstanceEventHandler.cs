using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceEventHandler(IModule thisModule) : 
    IEventHandler<HandledByHandlerInstanceEvent>
{
    private readonly IModule _thisModule = thisModule;

    public Task Handle(HandledByHandlerInstanceEvent eventData, CancellationToken cancellationToken)
    {
        _thisModule.Publish(eventData);
        return Task.CompletedTask;
    }
}
