using System.Threading;
using System.Threading.Tasks;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace Test.Integration.PassingMessagesBetweenSubmodules.SUT.Handlers;

internal class BetaGammaProxyHandler
    : IEventHandler<BetaEvent>
    , ICommandHandler<BetaCommand>
    , IQueryHandler<BetaQuery, string>
{
    private readonly IModule _gammaModule;

    public BetaGammaProxyHandler(ISubmodule<GammaModule> gammaModule)
    {
        _gammaModule = gammaModule;
    }

    public Task Handle(BetaEvent eventData, CancellationToken cancellationToken)
    {
        _gammaModule.PublishEvent<FromBetaEvent>(new(eventData.Id), cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(BetaCommand commandData, CancellationToken cancellationToken)
    {
        return _gammaModule.ExecuteCommand<FromBetaCommand>(new(commandData.Id), cancellationToken);
    }

    public Task<string> Handle(BetaQuery queryData, CancellationToken cancellationToken)
    {
        return _gammaModule.ExecuteQuery<FromBetaQuery, string>(new(queryData.Id), cancellationToken);
    }
}
