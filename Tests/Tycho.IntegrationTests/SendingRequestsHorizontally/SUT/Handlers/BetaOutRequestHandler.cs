using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class BetaOutRequestHandler(IModule<GammaModule> gammaModule)
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task HandleAsync(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync<GammaInRequestWithResponse, string>(
            new GammaInRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}