using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class AlphaInRequestHandler(IModule<GammaModule> gammaModule)
    : IHandle<BetaInRequest>
    , IHandle<BetaInRequestWithResponse, string>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task Handle(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.Execute(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.Execute<GammaInRequestWithResponse, string>(
            new GammaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
