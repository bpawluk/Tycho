using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta;
using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha.Handlers;

internal class GammaOutRequestHandler(IParentReference parent)
    : IRequestHandler<BetaOutRequest>
    , IRequestHandler<BetaOutRequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(BetaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new AlphaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(BetaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<AlphaOutRequestWithResponse, string>(
        //    new AlphaOutRequestWithResponse(requestData.Result),
        //    cancellationToken);
    }
}