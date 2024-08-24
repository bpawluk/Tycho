using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceRequestWithResponseHandler(IModule thisModule) : 
    IRequestHandler<HandledByHandlerInstanceRequestWithResponse, string>
{
    private readonly IModule _thisModule = thisModule;

    public Task<string> Handle(HandledByHandlerInstanceRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _thisModule.Execute<HandledByHandlerInstanceRequestWithResponse, string>(requestData);
    }
}
