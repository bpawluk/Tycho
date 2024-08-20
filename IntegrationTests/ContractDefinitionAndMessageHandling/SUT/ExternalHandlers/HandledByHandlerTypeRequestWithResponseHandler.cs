using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeRequestWithResponseHandler : IRequestHandler<HandledByHandlerTypeRequestWithResponse, string>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerTypeRequestWithResponseHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task<string> Handle(HandledByHandlerTypeRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tests.RequestWithResponseResponse);
    }
}
