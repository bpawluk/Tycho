using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Gamma;
using TychoV2.Requests;
using TychoV2.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Beta.Handlers;

internal class GammaOutRequestHandler(IParent parent)
    : IHandle<GammaOutRequest>
    , IHandle<GammaOutRequestWithResponse, string>
{
    private readonly IParent _parent = parent;

    public Task Handle(GammaOutRequest requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute(new BetaOutRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(GammaOutRequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _parent.Execute<BetaOutRequestWithResponse, string>(
            new BetaOutRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
