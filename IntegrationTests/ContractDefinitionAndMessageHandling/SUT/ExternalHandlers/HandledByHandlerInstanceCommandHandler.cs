using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceCommandHandler : IRequestHandler<HandledByHandlerInstanceCommand>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerInstanceCommandHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task Handle(HandledByHandlerInstanceCommand commandData, CancellationToken cancellationToken)
    {
        _tests.CommandWorkflowCompleted = true;
        return Task.CompletedTask;
    }
}
