using Tycho.Requests;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaInRequestHandler(IParent parent)
    : IRequestHandler<BetaInRequest>
    , IRequestHandler<BetaInRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task HandleAsync(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.ExecuteAsync<BetaOutRequestWithResponse, string>(
            new BetaOutRequestWithResponse(requestData.Result),
            cancellationToken);
    }
}