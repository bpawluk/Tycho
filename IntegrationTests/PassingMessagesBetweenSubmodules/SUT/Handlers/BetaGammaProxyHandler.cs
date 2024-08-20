using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Handlers;

internal class BetaGammaProxyHandler
    : IEventHandler<BetaEvent>
    , IRequestHandler<BetaRequest>
    , IRequestHandler<BetaRequestWithResponse, string>
{
    private readonly IModule _gammaModule;

    public BetaGammaProxyHandler(IModule<GammaModule> gammaModule)
    {
        _gammaModule = gammaModule;
    }

    public Task Handle(BetaEvent eventData, CancellationToken cancellationToken)
    {
        _gammaModule.Publish<FromBetaEvent>(new(eventData.Id), cancellationToken);
        return Task.CompletedTask;
    }

    public Task Handle(BetaRequest requestData, CancellationToken cancellationToken)
    {
        return _gammaModule.Execute<FromBetaRequest>(new(requestData.Id), cancellationToken);
    }

    public Task<string> Handle(BetaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _gammaModule.Execute<FromBetaRequestWithResponse, string>(new(requestData.Id), cancellationToken);
    }
}
