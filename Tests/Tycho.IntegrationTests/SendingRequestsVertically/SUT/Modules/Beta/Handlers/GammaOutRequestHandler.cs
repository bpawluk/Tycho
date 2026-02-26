using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using Tycho.Requests;
using Tycho.Structure.External;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class GammaOutRequestHandler(IParentReference parent)
    : IRequestHandler<GammaOutRequest>
    , IRequestHandler<GammaOutRequestWithResponse, string>
{
    private readonly IParentReference _parent = parent;

    public Task HandleAsync(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.CompletedTask;
        //return _parent.ExecuteAsync(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> HandleAsync(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return Task.FromResult("Error");
        //return _parent.ExecuteAsync<BetaOutRequestWithResponse, string>(
        //    new BetaOutRequestWithResponse(requestData.Result),
        //    cancellationToken);
    }
}