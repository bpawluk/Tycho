using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeRequestWithResponseHandler : IRequestHandler<HandledByHandlerTypeRequestWithResponse, string>
{
    public Task<string> Handle(HandledByHandlerTypeRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(ContractDefinitionAndMessageHandlingTests.ReturnedResponse);
    }
}
