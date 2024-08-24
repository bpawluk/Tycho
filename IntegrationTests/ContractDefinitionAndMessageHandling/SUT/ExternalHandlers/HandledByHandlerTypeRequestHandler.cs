using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeRequestHandler(ContractDefinitionAndMessageHandlingTests tests) : 
    IRequestHandler<HandledByHandlerTypeRequest>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests = tests;

    public Task Handle(HandledByHandlerTypeRequest requestData, CancellationToken cancellationToken)
    {
        _tests.RequestWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
