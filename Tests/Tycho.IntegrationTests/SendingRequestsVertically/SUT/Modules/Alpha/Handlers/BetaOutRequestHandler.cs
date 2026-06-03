using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;

internal class GammaOutRequestHandler(IAlphaModuleParent parent)
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IAlphaModuleParent _parent = parent;

    public Task HandleAsync(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new AlphaOutRequestWithResponse(requestData.Result), cancellationToken);
    }
}
