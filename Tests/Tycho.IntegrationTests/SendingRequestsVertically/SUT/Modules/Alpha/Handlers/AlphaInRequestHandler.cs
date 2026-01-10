using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IModuleInstance<BetaModule> betaModule)
    : IRequestHandler<AlphaInRequest>
    , IRequestHandler<AlphaInRequestWithResponse, string>
{
    private readonly IModuleInstance<BetaModule> _betaModule = betaModule;

    public Task HandleAsync(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.ExecuteAsync(new BetaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.ExecuteAsync<BetaInRequestWithResponse, string>(
            new BetaInRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}