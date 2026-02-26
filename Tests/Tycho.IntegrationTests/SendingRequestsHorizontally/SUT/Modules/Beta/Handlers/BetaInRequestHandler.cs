using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaInRequestHandler(IParentReference parent)
    : IRequestHandler<BetaInRequest>
    , IRequestHandler<BetaInRequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(BetaInRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaInRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<BetaOutRequestWithResponse, string>(
        //    new BetaOutRequestWithResponse(requestData.Result),
        //    cancellationToken);
    }
}