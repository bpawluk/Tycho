using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerTypeQueryHandler : IRequestHandler<HandledByHandlerTypeQuery, string>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerTypeQueryHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task<string> Handle(HandledByHandlerTypeQuery queryData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tests.QueryResponse);
    }
}
