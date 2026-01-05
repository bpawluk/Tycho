using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class GammaOutRequestHandler(IParent parent)
    : IRequestHandler<GammaOutRequest>
    , IRequestHandler<GammaOutRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync<BetaOutRequestWithResponse, string>(
            new BetaOutRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}