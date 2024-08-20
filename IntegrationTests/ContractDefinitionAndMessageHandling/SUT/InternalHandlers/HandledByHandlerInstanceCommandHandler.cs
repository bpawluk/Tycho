using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceCommandHandler : IRequestHandler<HandledByHandlerInstanceCommand>
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
