using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceRequestHandler : IRequestHandler<HandledByHandlerInstanceRequest>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerInstanceRequestHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task Handle(HandledByHandlerInstanceRequest requestData, CancellationToken cancellationToken)
    {
        _tests.RequestWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
