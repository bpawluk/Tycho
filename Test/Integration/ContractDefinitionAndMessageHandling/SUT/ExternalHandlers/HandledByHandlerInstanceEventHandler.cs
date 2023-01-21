using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceEventHandler : IEventHandler<HandledByHandlerInstanceEvent>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerInstanceEventHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task Handle(HandledByHandlerInstanceEvent eventData, CancellationToken cancellationToken)
    {
        _tests.EventWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
