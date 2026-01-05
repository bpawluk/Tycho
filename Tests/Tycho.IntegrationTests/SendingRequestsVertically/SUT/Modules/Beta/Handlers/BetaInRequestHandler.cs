using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class AlphaInRequestHandler(IModule<GammaModule> gammaModule)
    : IRequestHandler<BetaInRequest>
    , IRequestHandler<BetaInRequestWithResponse, string>
{
    private readonly IModule<GammaModule> _gammaModule = gammaModule;

    public Task HandleAsync(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync(new GammaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _gammaModule.ExecuteAsync<GammaInRequestWithResponse, string>(
            new GammaInRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}