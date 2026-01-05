using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Gamma.Handlers;

internal class GammaInRequestHandler(IParent parent)
    : IRequestHandler<GammaInRequest>
    , IRequestHandler<GammaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(GammaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new GammaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(GammaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync<GammaOutRequestWithResponse, string>(
            new GammaOutRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}