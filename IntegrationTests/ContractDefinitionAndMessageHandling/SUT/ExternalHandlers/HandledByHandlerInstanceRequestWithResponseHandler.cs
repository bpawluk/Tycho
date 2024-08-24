using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceRequestWithResponseHandler :  IRequestHandler<HandledByHandlerInstanceRequestWithResponse, string>
{
    public Task<string> Handle(HandledByHandlerInstanceRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(ContractDefinitionAndMessageHandlingTests.ReturnedResponse);
    }
}
