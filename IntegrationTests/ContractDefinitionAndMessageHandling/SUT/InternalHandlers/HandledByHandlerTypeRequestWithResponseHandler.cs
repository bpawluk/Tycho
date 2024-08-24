using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeRequestWithResponseHandler(IModule thisModule) : 
    IRequestHandler<HandledByHandlerTypeRequestWithResponse, string>
{
    private readonly IModule _thisModule = thisModule;

    public Task<string> Handle(HandledByHandlerTypeRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _thisModule.Execute<HandledByHandlerTypeRequestWithResponse, string>(requestData);
    }
}
