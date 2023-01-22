using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeQueryHandler : IQueryHandler<HandledByHandlerTypeQuery, string>
{
    private readonly IModule _thisModule;

    public HandledByHandlerTypeQueryHandler(IModule thisModule)
    {
        _thisModule = thisModule;
    }

    public Task<string> Handle(HandledByHandlerTypeQuery queryData, CancellationToken cancellationToken = default)
    {
        return _thisModule.Execute<HandledByHandlerTypeQuery, string>(queryData);
    }
}
