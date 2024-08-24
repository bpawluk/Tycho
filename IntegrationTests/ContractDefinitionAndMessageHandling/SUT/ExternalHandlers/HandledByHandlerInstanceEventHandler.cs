using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceEventHandler(ContractDefinitionAndMessageHandlingTests tests) : 
    IEventHandler<HandledByHandlerInstanceEvent>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests = tests;

    public Task Handle(HandledByHandlerInstanceEvent eventData, CancellationToken cancellationToken)
    {
        _tests.EventWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
