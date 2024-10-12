using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Handlers;

internal class AlphaOutRequestHandler(IModule<BetaModule> betaModule)
    : IRequestHandler<AlphaOutRequest>
    , IRequestHandler<AlphaOutRequestWithResponse, string>
{
    private readonly IModule<BetaModule> _betaModule = betaModule;

    public Task Handle(AlphaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.Execute(new BetaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(AlphaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.Execute<BetaInRequestWithResponse, string>(
            new BetaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
