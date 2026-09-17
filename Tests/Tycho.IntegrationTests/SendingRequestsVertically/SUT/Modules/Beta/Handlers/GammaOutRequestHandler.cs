using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class GammaOutRequestHandler(IBetaModuleParent parent)
    : IRequestHandler<GammaOutRequest>
    , IRequestHandler<GammaOutRequestWithResponse, string>
{
    private readonly IBetaModuleParent _parent = parent;

    public Task HandleAsync(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new BetaOutRequestWithResponse(requestData.Result), cancellationToken);
    }
}
