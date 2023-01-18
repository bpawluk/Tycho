using System.Threading;
using System.Threading.Tasks;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.PassingMessagesBetweenSubmodules.SUT.Handlers;

internal class AlphaBetaProxyHandler
    : IEventHandler<AlphaEvent>
    , ICommandHandler<AlphaCommand>
    , IQueryHandler<AlphaQuery, string>
{
    private readonly IModule _betaModule;

    public AlphaBetaProxyHandler(ISubmodule<BetaModule> betaModule)
    {
        _betaModule = betaModule;
    }

    public Task Handle(AlphaEvent eventData, CancellationToken cancellationToken)
    {
        _betaModule.PublishEvent<FromAlphaEvent>(new(eventData.Id), cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(AlphaCommand commandData, CancellationToken cancellationToken)
    {
        return _betaModule.ExecuteCommand<FromAlphaCommand>(new(commandData.Id), cancellationToken);
    }

    public Task<string> Handle(AlphaQuery queryData, CancellationToken cancellationToken)
    {
        return _betaModule.ExecuteQuery<FromAlphaQuery, string>(new(queryData.Id), cancellationToken);
    }
}
