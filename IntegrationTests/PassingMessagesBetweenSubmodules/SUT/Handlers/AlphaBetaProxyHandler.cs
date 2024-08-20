using System.Threading;
using System.Threading.Tasks;
using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Handlers;

internal class AlphaBetaProxyHandler
    : IEventHandler<AlphaEvent>
    , IRequestHandler<AlphaRequest>
    , IRequestHandler<AlphaRequestWithResponse, string>
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

    public Task Handle(AlphaRequest requestData, CancellationToken cancellationToken)
    {
        return _betaModule.Execute<FromAlphaRequest>(new(requestData.Id), cancellationToken);
    }

    public Task<string> Handle(AlphaRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        return _betaModule.Execute<FromAlphaRequestWithResponse, string>(new(requestData.Id), cancellationToken);
    }
}
