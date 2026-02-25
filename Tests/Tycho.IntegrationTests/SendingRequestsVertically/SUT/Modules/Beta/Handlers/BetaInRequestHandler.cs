using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class AlphaInRequestHandler(IGammaModule gammaModule)
    : IRequestHandler<BetaInRequest>
    , IRequestHandler<BetaInRequestWithResponse, string>
{
    private readonly IGammaModule _gammaModule = gammaModule;

    public Task HandleAsync(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequestWithResponse(requestData.Result), cancellationToken);
    }
}