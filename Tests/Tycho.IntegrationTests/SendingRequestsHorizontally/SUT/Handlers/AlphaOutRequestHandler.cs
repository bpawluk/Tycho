using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class AlphaOutRequestHandler(IModule<BetaModule> betaModule)
    : IRequestHandler<AlphaOutRequest>
    , IRequestHandler<AlphaOutRequestWithResponse, string>
{
    private readonly IModule<BetaModule> _betaModule = betaModule;

    public Task HandleAsync(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.ExecuteAsync(new BetaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.ExecuteAsync<BetaInRequestWithResponse, string>(
            new BetaInRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}