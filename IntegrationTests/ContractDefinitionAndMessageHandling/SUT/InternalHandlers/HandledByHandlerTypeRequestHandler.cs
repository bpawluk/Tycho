using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeRequestHandler : IRequestHandler<HandledByHandlerTypeRequest>
{
    private readonly IModule _thisModule;

    public HandledByHandlerTypeRequestHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerTypeRequest requestData, CancellationToken cancellationToken)
    {
        _thisModule.Execute(requestData);
        return Task.CompletedTask;
    }
}
