using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;

internal class AlphaInRequestHandler(IModule<BetaModule> betaModule)
    : IHandle<AlphaInRequest>
    , IHandle<AlphaInRequestWithResponse, string>
{
    private readonly IModule<BetaModule> _betaModule = betaModule;

    public Task Handle(AlphaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.Execute(new BetaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(AlphaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _betaModule.Execute<BetaInRequestWithResponse, string>(
            new BetaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
