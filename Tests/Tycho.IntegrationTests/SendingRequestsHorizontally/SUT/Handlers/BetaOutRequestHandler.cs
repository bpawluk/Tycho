using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class BetaOutRequestHandler(IGammaModule gammaModule)
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IGammaModule _gammaModule = gammaModule;

    public Task HandleAsync(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequestWithResponse(requestData.Result), cancellationToken);
    }
}