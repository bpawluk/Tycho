using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerInstanceQueryHandler : IQueryHandler<HandledByHandlerInstanceQuery, string>
{
    private readonly IModule _thisModule;

    public HandledByHandlerInstanceQueryHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task<string> Handle(HandledByHandlerInstanceQuery queryData, CancellationToken cancellationToken = default)
    {
        return _thisModule.ExecuteQuery<HandledByHandlerInstanceQuery, string>(queryData);
    }
}
