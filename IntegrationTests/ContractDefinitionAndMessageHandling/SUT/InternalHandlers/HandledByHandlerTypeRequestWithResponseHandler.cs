using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeRequestWithResponseHandler : IRequestHandler<HandledByHandlerTypeRequestWithResponse, string>
{
    private readonly IModule _thisModule;

    public HandledByHandlerTypeRequestWithResponseHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task<string> Handle(HandledByHandlerTypeRequestWithResponse requestData, CancellationToken cancellationToken = default)
    {
        return _thisModule.Execute<HandledByHandlerTypeRequestWithResponse, string>(requestData);
    }
}
