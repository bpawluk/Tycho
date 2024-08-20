using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Handlers;

internal class AlphaBetaProxyHandler
    : IEventHandler<AlphaEvent>
    , IRequestHandler<AlphaCommand>
    , IRequestHandler<AlphaQuery, string>
{
    private readonly IModule _betaModule;

    public AlphaBetaProxyHandler(IModule<BetaModule> betaModule)
    {
        _betaModule = betaModule;
    }

    public Task Handle(AlphaEvent eventData, CancellationToken cancellationToken)
    {
        _betaModule.Publish<FromAlphaEvent>(new(eventData.Id), cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaCommand commandData, CancellationToken cancellationToken)
    {
        return _betaModule.Execute<FromAlphaCommand>(new(commandData.Id), cancellationToken);
    }

    public Task<string> Handle(AlphaQuery queryData, CancellationToken cancellationToken)
    {
        return _betaModule.Execute<FromAlphaQuery, string>(new(queryData.Id), cancellationToken);
    }
}
