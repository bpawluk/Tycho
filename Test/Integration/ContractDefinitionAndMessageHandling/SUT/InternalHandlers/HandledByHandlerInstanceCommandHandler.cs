using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceCommandHandler : ICommandHandler<HandledByHandlerInstanceCommand>
{
    private readonly IModule _thisModule;

    public HandledByHandlerInstanceCommandHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerInstanceCommand commandData, CancellationToken cancellationToken)
    {
        _thisModule.Execute(commandData);
        return Task.CompletedTask;
    }
}
