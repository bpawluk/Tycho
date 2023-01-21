using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceEventHandler : IEventHandler<HandledByHandlerInstanceEvent>
{
    private readonly IModule _thisModule;

    public HandledByHandlerInstanceEventHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerInstanceEvent eventData, CancellationToken cancellationToken)
    {
        _thisModule.PublishEvent(eventData);
        return Task.CompletedTask;
    }
}
