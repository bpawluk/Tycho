using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ContractDefinitionAndMessageHandling.SUT.InternalHandlers;

internal class HandledByHandlerTypeQueryHandler : IRequestHandler<HandledByHandlerTypeQuery, string>
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
