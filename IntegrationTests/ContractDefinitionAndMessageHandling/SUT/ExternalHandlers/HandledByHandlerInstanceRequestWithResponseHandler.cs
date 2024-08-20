using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceRequestWithResponseHandler : IRequestHandler<HandledByHandlerInstanceRequestWithResponse, string>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerInstanceRequestWithResponseHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task<string> Handle(HandledByHandlerInstanceRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tests.RequestWithResponseResponse);
    }
}
