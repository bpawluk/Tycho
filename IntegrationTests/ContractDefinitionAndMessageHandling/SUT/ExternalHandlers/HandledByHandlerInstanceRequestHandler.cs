using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceRequestHandler(ContractDefinitionAndMessageHandlingTests tests) : 
    IRequestHandler<HandledByHandlerInstanceRequest>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests = tests;

    public Task Handle(HandledByHandlerInstanceRequest requestData, CancellationToken cancellationToken)
    {
        _tests.RequestWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
