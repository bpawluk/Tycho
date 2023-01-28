using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceQueryHandler : IQueryHandler<HandledByHandlerInstanceQuery, string>
{
    private readonly IModule _thisModule;

    public HandledByHandlerInstanceQueryHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task<string> Handle(HandledByHandlerInstanceQuery queryData, CancellationToken cancellationToken = default)
    {
        return _thisModule.Execute<HandledByHandlerInstanceQuery, string>(queryData);
    }
}
