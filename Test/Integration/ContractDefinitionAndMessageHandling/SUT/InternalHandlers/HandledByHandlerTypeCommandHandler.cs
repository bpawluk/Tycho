using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeCommandHandler : ICommandHandler<HandledByHandlerTypeCommand>
{
    private readonly IModule _thisModule;

    public HandledByHandlerTypeCommandHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerTypeCommand commandData, CancellationToken cancellationToken)
    {
        _thisModule.ExecuteCommand(commandData);
        return Task.CompletedTask;
    }
}
