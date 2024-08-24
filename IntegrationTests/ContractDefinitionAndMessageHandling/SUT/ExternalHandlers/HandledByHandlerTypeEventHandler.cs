using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeEventHandler(ContractDefinitionAndMessageHandlingTests tests) : 
    IEventHandler<HandledByHandlerTypeEvent>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests = tests;

    public Task Handle(HandledByHandlerTypeEvent eventData, CancellationToken cancellationToken)
    {
        _tests.EventWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
