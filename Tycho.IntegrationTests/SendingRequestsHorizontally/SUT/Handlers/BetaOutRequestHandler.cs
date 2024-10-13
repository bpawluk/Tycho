using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class BetaOutRequestHandler(IModule<GammaModule> gammaModule) 
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task Handle(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.Execute(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.Execute<GammaInRequestWithResponse, string>(
            new GammaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
