using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.ExternalHandlers;

internal class HandledByHandlerInstanceQueryHandler : IRequestHandler<HandledByHandlerInstanceQuery, string>
{
    private readonly ContractDefinitionAndMessageHandlingTests _tests;

    public HandledByHandlerInstanceQueryHandler(ContractDefinitionAndMessageHandlingTests tests)
    {
        _tests = tests;
    }

    public Task<string> Handle(HandledByHandlerInstanceQuery queryData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tests.QueryResponse);
    }
}
