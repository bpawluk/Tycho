using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeEventHandler : IEventHandler<HandledByHandlerTypeEvent>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerTypeEventHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task Handle(HandledByHandlerTypeEvent eventData, CancellationToken cancellationToken)
    {
        _tests.EventWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
