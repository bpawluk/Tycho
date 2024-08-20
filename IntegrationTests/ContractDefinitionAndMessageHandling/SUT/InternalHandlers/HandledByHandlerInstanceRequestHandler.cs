using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceRequestHandler : IRequestHandler<HandledByHandlerInstanceRequest>
{
    private readonly IModule _thisModule;

    public HandledByHandlerInstanceRequestHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task Handle(HandledByHandlerInstanceRequest requestData, CancellationToken cancellationToken)
    {
        _thisModule.Execute(requestData);
        return Task.CompletedTask;
    }
}
